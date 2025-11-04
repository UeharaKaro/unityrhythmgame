using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// .sheet 파일을 osu!mania 4K .osu 파일로 변환하는 클래스
/// </summary>
public class SheetToOsuConverter
{
    private const int MANIA_KEY_COUNT = 4;
    // 4K 모드에서 각 레인의 x 좌표 (512를 4등분한 각 구간의 중앙)
    private static readonly int[] LANE_X_POSITIONS = { 64, 192, 320, 448 };

    /// <summary>
    /// Sheet 객체를 .osu 파일 형식의 문자열로 변환
    /// </summary>
    public static string ConvertSheetToOsu(Sheet sheet, string audioFileName = "audio.mp3", string creator = "Unknown", string version = "4K")
    {
        StringBuilder sb = new StringBuilder();

        // 파일 포맷 버전
        sb.AppendLine("osu file format v14");
        sb.AppendLine();

        // [General] 섹션
        sb.AppendLine("[General]");
        sb.AppendLine($"AudioFilename: {audioFileName}");
        sb.AppendLine("AudioLeadIn: 0");
        sb.AppendLine("PreviewTime: -1");
        sb.AppendLine("Countdown: 0");
        sb.AppendLine("SampleSet: Normal");
        sb.AppendLine("StackLeniency: 0.7");
        sb.AppendLine("Mode: 3"); // 3 = osu!mania
        sb.AppendLine("LetterboxInBreaks: 0");
        sb.AppendLine("SpecialStyle: 0");
        sb.AppendLine("WidescreenStoryboard: 0");
        sb.AppendLine();

        // [Editor] 섹션
        sb.AppendLine("[Editor]");
        sb.AppendLine("DistanceSpacing: 1");
        sb.AppendLine("BeatDivisor: 4");
        sb.AppendLine("GridSize: 4");
        sb.AppendLine("TimelineZoom: 1");
        sb.AppendLine();

        // [Metadata] 섹션
        sb.AppendLine("[Metadata]");
        sb.AppendLine($"Title:{sheet.title}");
        sb.AppendLine($"TitleUnicode:{sheet.title}");
        sb.AppendLine($"Artist:{sheet.artist}");
        sb.AppendLine($"ArtistUnicode:{sheet.artist}");
        sb.AppendLine($"Creator:{creator}");
        sb.AppendLine($"Version:{version}");
        sb.AppendLine("Source:");
        sb.AppendLine("Tags:");
        sb.AppendLine("BeatmapID:0");
        sb.AppendLine("BeatmapSetID:-1");
        sb.AppendLine();

        // [Difficulty] 섹션
        sb.AppendLine("[Difficulty]");
        sb.AppendLine("HPDrainRate:8");
        sb.AppendLine($"CircleSize:{MANIA_KEY_COUNT}"); // 4K
        sb.AppendLine("OverallDifficulty:8");
        sb.AppendLine("ApproachRate:5");
        sb.AppendLine("SliderMultiplier:1.4");
        sb.AppendLine("SliderTickRate:1");
        sb.AppendLine();

        // [Events] 섹션
        sb.AppendLine("[Events]");
        sb.AppendLine("//Background and Video events");
        sb.AppendLine("//Break Periods");
        sb.AppendLine("//Storyboard Layer 0 (Background)");
        sb.AppendLine("//Storyboard Layer 1 (Fail)");
        sb.AppendLine("//Storyboard Layer 2 (Pass)");
        sb.AppendLine("//Storyboard Layer 3 (Foreground)");
        sb.AppendLine("//Storyboard Layer 4 (Overlay)");
        sb.AppendLine("//Storyboard Sound Samples");
        sb.AppendLine();

        // [TimingPoints] 섹션
        sb.AppendLine("[TimingPoints]");
        double msPerBeat = 60000.0 / sheet.bpm;
        int meter = sheet.signature[0];
        // offset, msPerBeat, meter, sampleSet, sampleIndex, volume, uninherited, effects
        sb.AppendLine($"{sheet.offset},{msPerBeat},{meter},1,0,100,1,0");
        sb.AppendLine();

        // [HitObjects] 섹션
        sb.AppendLine("[HitObjects]");

        foreach (Note note in sheet.notes.OrderBy(n => n.time))
        {
            // 레인 번호를 x 좌표로 변환 (1-based -> 0-based)
            int laneIndex = note.line - 1;
            if (laneIndex < 0) laneIndex = 0;
            if (laneIndex >= MANIA_KEY_COUNT) laneIndex = MANIA_KEY_COUNT - 1;

            int x = LANE_X_POSITIONS[laneIndex];
            int y = 192; // mania에서는 y값이 무시되지만 관례상 192 사용

            if (note.type == (int)NoteType.Short)
            {
                // 일반 노트: x,y,time,type,hitSound,hitSample
                // type = 1 (circle)
                sb.AppendLine($"{x},{y},{note.time},1,0,0:0:0:0:");
            }
            else if (note.type == (int)NoteType.Long)
            {
                // 롱노트: x,y,time,type,hitSound,endTime:hitSample
                // type = 128 (long note)
                sb.AppendLine($"{x},{y},{note.time},128,0,{note.tail}:0:0:0:0:");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// .sheet 파일을 읽어서 .osu 파일로 저장
    /// </summary>
    public static void ConvertAndSave(string sheetFilePath, string osuFilePath, string audioFileName = "audio.mp3", string creator = "Unknown", string version = "4K")
    {
        // .sheet 파일을 읽어서 Sheet 객체 생성
        Sheet sheet = ParseSheetFile(sheetFilePath);

        // .osu 파일 내용 생성
        string osuContent = ConvertSheetToOsu(sheet, audioFileName, creator, version);

        // .osu 파일 저장
        File.WriteAllText(osuFilePath, osuContent, Encoding.UTF8);

        UnityEngine.Debug.Log($"변환 완료: {sheetFilePath} -> {osuFilePath}");
    }

    /// <summary>
    /// .sheet 파일을 읽어서 Sheet 객체로 파싱
    /// </summary>
    public static Sheet ParseSheetFile(string sheetFilePath)
    {
        if (!File.Exists(sheetFilePath))
        {
            throw new FileNotFoundException($"sheet 파일을 찾을 수 없습니다: {sheetFilePath}");
        }

        Sheet sheet = new Sheet();
        List<Note> notes = new List<Note>();
        string currentSection = "";

        using (StreamReader sr = new StreamReader(sheetFilePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();

                // 빈 줄 무시
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // 섹션 확인
                if (line.StartsWith("["))
                {
                    currentSection = line;
                    continue;
                }

                // Description 섹션 파싱
                if (currentSection == "[Description]")
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
                // Audio 섹션 파싱
                else if (currentSection == "[Audio]")
                {
                    if (line.StartsWith("BPM:"))
                    {
                        sheet.bpm = int.Parse(line.Substring(4).Trim());
                    }
                    else if (line.StartsWith("Offset:"))
                    {
                        sheet.offset = int.Parse(line.Substring(7).Trim());
                    }
                    else if (line.StartsWith("Signature:"))
                    {
                        string[] parts = line.Substring(10).Trim().Split('/');
                        sheet.signature = new int[] { int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()) };
                    }
                }
                // Note 섹션 파싱
                else if (currentSection == "[Note]")
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        int time = int.Parse(parts[0].Trim());
                        int type = int.Parse(parts[1].Trim());
                        int laneNum = int.Parse(parts[2].Trim());
                        int tail = -1;

                        if (parts.Length >= 4)
                        {
                            tail = int.Parse(parts[3].Trim());
                        }

                        notes.Add(new Note(time, type, laneNum, tail));
                    }
                }
            }
        }

        sheet.notes = notes.OrderBy(n => n.time).ToList();
        sheet.Init();

        return sheet;
    }
}
