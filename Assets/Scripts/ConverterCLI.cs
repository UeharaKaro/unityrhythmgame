using System;
using System.IO;

/// <summary>
/// 커맨드라인에서 사용할 수 있는 변환기 인터페이스
/// Unity 에디터의 메뉴나 스크립트에서 호출 가능
/// </summary>
public class ConverterCLI
{
    /// <summary>
    /// 커맨드라인 인터페이스 메인 함수
    /// </summary>
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();
        string inputFile = args[1];

        try
        {
            switch (command)
            {
                case "sheet2osu":
                case "s2o":
                    ConvertSheetToOsu(args);
                    break;

                case "osu2sheet":
                case "o2s":
                    ConvertOsuToSheet(args);
                    break;

                case "test":
                    RunTest(inputFile);
                    break;

                default:
                    Console.WriteLine($"알 수 없는 명령어: {command}");
                    PrintUsage();
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"오류 발생: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
    }

    private static void ConvertSheetToOsu(string[] args)
    {
        string sheetFile = args[1];
        string osuFile = args.Length > 2 ? args[2] : Path.ChangeExtension(sheetFile, ".osu");
        string audioFile = args.Length > 3 ? args[3] : "audio.mp3";
        string creator = args.Length > 4 ? args[4] : "Unknown";
        string version = args.Length > 5 ? args[5] : "4K";

        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Sheet → Osu 변환");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"입력: {sheetFile}");
        Console.WriteLine($"출력: {osuFile}");
        Console.WriteLine($"오디오: {audioFile}");
        Console.WriteLine($"제작자: {creator}");
        Console.WriteLine($"난이도: {version}");
        Console.WriteLine();

        SheetToOsuConverter.ConvertAndSave(sheetFile, osuFile, audioFile, creator, version);

        Console.WriteLine("✓ 변환 완료!");
        Console.WriteLine($"생성된 파일: {osuFile}");
    }

    private static void ConvertOsuToSheet(string[] args)
    {
        string osuFile = args[1];
        string sheetFile = args.Length > 2 ? args[2] : Path.ChangeExtension(osuFile, ".sheet");

        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Osu → Sheet 변환");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"입력: {osuFile}");
        Console.WriteLine($"출력: {sheetFile}");
        Console.WriteLine();

        OsuToSheetConverter.ConvertAndSave(osuFile, sheetFile);

        Console.WriteLine("✓ 변환 완료!");
        Console.WriteLine($"생성된 파일: {sheetFile}");
    }

    private static void RunTest(string sheetFile)
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("양방향 변환 테스트");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"테스트 파일: {sheetFile}");
        Console.WriteLine();

        if (!File.Exists(sheetFile))
        {
            Console.WriteLine($"파일을 찾을 수 없습니다: {sheetFile}");
            return;
        }

        // 1. 원본 sheet 읽기
        Console.WriteLine("1단계: 원본 .sheet 파일 읽기...");
        string originalContent = File.ReadAllText(sheetFile);
        Sheet originalSheet = SheetToOsuConverter.ParseSheetFile(sheetFile);
        Console.WriteLine($"  ✓ 노트 개수: {originalSheet.notes.Count}");
        Console.WriteLine($"  ✓ BPM: {originalSheet.bpm}");
        Console.WriteLine($"  ✓ Offset: {originalSheet.offset}ms");
        Console.WriteLine();

        // 2. sheet -> osu 변환
        Console.WriteLine("2단계: .sheet → .osu 변환...");
        string tempOsuFile = Path.Combine(Path.GetDirectoryName(sheetFile), Path.GetFileNameWithoutExtension(sheetFile) + "_test.osu");
        SheetToOsuConverter.ConvertAndSave(sheetFile, tempOsuFile, "audio.mp3", "Test", "4K");
        Console.WriteLine($"  ✓ 생성: {tempOsuFile}");
        Console.WriteLine();

        // 3. osu -> sheet 변환
        Console.WriteLine("3단계: .osu → .sheet 변환...");
        string tempSheetFile = Path.Combine(Path.GetDirectoryName(sheetFile), Path.GetFileNameWithoutExtension(sheetFile) + "_reconverted.sheet");
        OsuToSheetConverter.ConvertAndSave(tempOsuFile, tempSheetFile);
        Console.WriteLine($"  ✓ 생성: {tempSheetFile}");
        Console.WriteLine();

        // 4. 결과 비교
        Console.WriteLine("4단계: 노트 데이터 비교...");
        Sheet reconvertedSheet = SheetToOsuConverter.ParseSheetFile(tempSheetFile);

        bool identical = true;
        int errorCount = 0;

        if (originalSheet.notes.Count != reconvertedSheet.notes.Count)
        {
            Console.WriteLine($"  ✗ 노트 개수 불일치! 원본: {originalSheet.notes.Count}, 변환: {reconvertedSheet.notes.Count}");
            identical = false;
        }
        else
        {
            Console.WriteLine($"  ✓ 노트 개수 일치: {originalSheet.notes.Count}");
        }

        int minCount = Math.Min(originalSheet.notes.Count, reconvertedSheet.notes.Count);
        for (int i = 0; i < minCount; i++)
        {
            Note orig = originalSheet.notes[i];
            Note conv = reconvertedSheet.notes[i];

            if (orig.time != conv.time || orig.type != conv.type || orig.line != conv.line || orig.tail != conv.tail)
            {
                if (errorCount < 5) // 처음 5개 에러만 출력
                {
                    Console.WriteLine($"  ✗ 노트 #{i + 1} 불일치!");
                    Console.WriteLine($"    원본: time={orig.time}, type={orig.type}, line={orig.line}, tail={orig.tail}");
                    Console.WriteLine($"    변환: time={conv.time}, type={conv.type}, line={conv.line}, tail={conv.tail}");
                }
                errorCount++;
                identical = false;
            }
        }

        if (errorCount > 5)
        {
            Console.WriteLine($"  ... 외 {errorCount - 5}개 에러");
        }

        Console.WriteLine();
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        if (identical)
        {
            Console.WriteLine("✓✓✓ 테스트 성공! ✓✓✓");
            Console.WriteLine("모든 노트가 완벽하게 일치합니다.");
        }
        else
        {
            Console.WriteLine("✗✗✗ 테스트 실패 ✗✗✗");
            Console.WriteLine($"총 {errorCount}개의 불일치 발견");
        }
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    private static void PrintUsage()
    {
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Sheet ↔ Osu 변환기");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine();
        Console.WriteLine("사용법:");
        Console.WriteLine();
        Console.WriteLine("  Sheet → Osu 변환:");
        Console.WriteLine("    ConverterCLI sheet2osu <sheet파일> [osu파일] [오디오파일] [제작자] [난이도]");
        Console.WriteLine("    ConverterCLI s2o <sheet파일>");
        Console.WriteLine();
        Console.WriteLine("  Osu → Sheet 변환:");
        Console.WriteLine("    ConverterCLI osu2sheet <osu파일> [sheet파일]");
        Console.WriteLine("    ConverterCLI o2s <osu파일>");
        Console.WriteLine();
        Console.WriteLine("  테스트:");
        Console.WriteLine("    ConverterCLI test <sheet파일>");
        Console.WriteLine();
        Console.WriteLine("예시:");
        Console.WriteLine("  ConverterCLI s2o song.sheet song.osu audio.mp3 Mapper Normal");
        Console.WriteLine("  ConverterCLI o2s beatmap.osu song.sheet");
        Console.WriteLine("  ConverterCLI test Consolation.sheet");
        Console.WriteLine();
    }
}
