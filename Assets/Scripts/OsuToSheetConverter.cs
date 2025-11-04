using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// osu!mania 4K .osu 파일을 .sheet 파일로 변환하는 클래스
/// </summary>
public class OsuToSheetConverter
{
    private const int MANIA_KEY_COUNT = 4;

    /// <summary>
    /// .osu 파일을 읽어서 Sheet 객체로 변환
    /// </summary>
    public static Sheet ConvertOsuToSheet(string osuFilePath)
    {
        if (!File.Exists(osuFilePath))
        {
            throw new FileNotFoundException($"osu 파일을 찾을 수 없습니다: {osuFilePath}");
        }

        Sheet sheet = new Sheet();
        List<Note> notes = new List<Note>();

        string currentSection = "";

        using (StreamReader sr = new StreamReader(osuFilePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();

                // 섹션 확인
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line;
                    continue;
                }

                // 빈 줄이나 주석 무시
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue;
                }

                // Metadata 섹션 파싱
                if (currentSection == "[Metadata]")
                {
                    if (line.StartsWith("Title:"))
                    {
                        sheet.title = line.Substring(6).Trim();
                    }
                    else if (line.StartsWith("Artist:"))
                    {
                        sheet.artist = line.Substring(7).Trim();
                    }
                }
                // Difficulty 섹션 파싱
                else if (currentSection == "[Difficulty]")
                {
                    if (line.StartsWith("CircleSize:"))
                    {
                        int circleSize = int.Parse(line.Substring(11).Trim());
                        if (circleSize != MANIA_KEY_COUNT)
                        {
                            UnityEngine.Debug.LogWarning($"이 변환기는 4K 전용입니다. CircleSize: {circleSize}");
                        }
                    }
                }
                // TimingPoints 섹션 파싱 (첫 번째 타이밍 포인트에서 BPM과 Offset 추출)
                else if (currentSection == "[TimingPoints]")
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        // uninherited (빨간선) 타이밍 포인트만 처리
                        if (parts.Length < 7 || parts[6].Trim() == "1")
                        {
                            if (sheet.offset == 0) // 첫 번째 타이밍 포인트만 사용
                            {
                                sheet.offset = (int)Math.Round(double.Parse(parts[0].Trim()));
                                double msPerBeat = double.Parse(parts[1].Trim());
                                sheet.bpm = (int)Math.Round(60000.0 / msPerBeat);

                                if (parts.Length >= 3)
                                {
                                    int meter = int.Parse(parts[2].Trim());
                                    sheet.signature = new int[] { meter, 4 }; // 보통 x/4 박자
                                }
                                else
                                {
                                    sheet.signature = new int[] { 4, 4 }; // 기본값 4/4
                                }
                            }
                        }
                    }
                }
                // HitObjects 섹션 파싱
                else if (currentSection == "[HitObjects]")
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        int x = int.Parse(parts[0].Trim());
                        int time = int.Parse(parts[2].Trim());
                        int type = int.Parse(parts[3].Trim());

                        // x 좌표를 레인 번호로 변환 (4K 기준)
                        // 0-127: Lane 1, 128-255: Lane 2, 256-383: Lane 3, 384-511: Lane 4
                        int lane = (x * MANIA_KEY_COUNT / 512) + 1;
                        if (lane > MANIA_KEY_COUNT) lane = MANIA_KEY_COUNT;
                        if (lane < 1) lane = 1;

                        // type & 128 = 롱노트, type & 1 = 일반 노트
                        if ((type & 128) != 0) // 롱노트
                        {
                            if (parts.Length >= 6)
                            {
                                string[] endParams = parts[5].Split(':');
                                int endTime = int.Parse(endParams[0].Trim());
                                notes.Add(new Note(time, (int)NoteType.Long, lane, endTime));
                            }
                        }
                        else // 일반 노트
                        {
                            notes.Add(new Note(time, (int)NoteType.Short, lane, -1));
                        }
                    }
                }
            }
        }

        // 노트를 시간 순으로 정렬
        sheet.notes = notes.OrderBy(n => n.time).ToList();

        // Sheet 초기화
        sheet.Init();

        return sheet;
    }

    /// <summary>
    /// .osu 파일을 읽어서 .sheet 파일로 저장
    /// </summary>
    public static void ConvertAndSave(string osuFilePath, string sheetFilePath)
    {
        Sheet sheet = ConvertOsuToSheet(osuFilePath);

        // .sheet 파일 작성
        using (StreamWriter sw = new StreamWriter(sheetFilePath))
        {
            sw.WriteLine("[Description]");
            sw.WriteLine($"Title: {sheet.title}");
            sw.WriteLine($"Artist: {sheet.artist}");
            sw.WriteLine();

            sw.WriteLine("[Audio]");
            sw.WriteLine($"BPM: {sheet.bpm}");
            sw.WriteLine($"Offset: {sheet.offset}");
            sw.WriteLine($"Signature: {sheet.signature[0]}/{sheet.signature[1]}");
            sw.WriteLine();

            sw.WriteLine("[Note]");
            foreach (Note note in sheet.notes)
            {
                if (note.type == (int)NoteType.Short)
                {
                    sw.WriteLine($"{note.time}, {note.type}, {note.line}");
                }
                else if (note.type == (int)NoteType.Long)
                {
                    sw.WriteLine($"{note.time}, {note.type}, {note.line}, {note.tail}");
                }
            }
        }

        UnityEngine.Debug.Log($"변환 완료: {osuFilePath} -> {sheetFilePath}");
    }
}
