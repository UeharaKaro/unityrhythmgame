using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Unity 에디터에서 .sheet와 .osu 파일을 변환하는 메뉴
/// </summary>
public class SheetOsuConverterEditor : EditorWindow
{
    private string sheetFilePath = "";
    private string osuFilePath = "";
    private string audioFileName = "audio.mp3";
    private string creator = "Mapper";
    private string version = "4K";

    [MenuItem("Tools/Sheet <-> Osu Converter")]
    public static void ShowWindow()
    {
        GetWindow<SheetOsuConverterEditor>("Sheet ↔ Osu Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sheet ↔ Osu 파일 변환기", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // .sheet to .osu 변환
        GUILayout.Label("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", EditorStyles.boldLabel);
        GUILayout.Label(".sheet → .osu 변환", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        sheetFilePath = EditorGUILayout.TextField("Sheet 파일:", sheetFilePath);
        if (GUILayout.Button("찾기", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("Sheet 파일 선택", Application.dataPath, "sheet");
            if (!string.IsNullOrEmpty(path))
            {
                sheetFilePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        audioFileName = EditorGUILayout.TextField("오디오 파일명:", audioFileName);
        creator = EditorGUILayout.TextField("제작자:", creator);
        version = EditorGUILayout.TextField("난이도명:", version);

        EditorGUILayout.Space();

        if (GUILayout.Button("Sheet → Osu 변환", GUILayout.Height(30)))
        {
            ConvertSheetToOsu();
        }

        EditorGUILayout.Space(20);

        // .osu to .sheet 변환
        GUILayout.Label("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", EditorStyles.boldLabel);
        GUILayout.Label(".osu → .sheet 변환", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        osuFilePath = EditorGUILayout.TextField("Osu 파일:", osuFilePath);
        if (GUILayout.Button("찾기", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("Osu 파일 선택", "", "osu");
            if (!string.IsNullOrEmpty(path))
            {
                osuFilePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Osu → Sheet 변환", GUILayout.Height(30)))
        {
            ConvertOsuToSheet();
        }

        EditorGUILayout.Space(20);

        // 빠른 테스트
        GUILayout.Label("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", EditorStyles.boldLabel);
        GUILayout.Label("빠른 테스트", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("기존 Sheet 파일 테스트 (Consolation)", GUILayout.Height(30)))
        {
            TestConversion();
        }
    }

    private void ConvertSheetToOsu()
    {
        if (string.IsNullOrEmpty(sheetFilePath) || !File.Exists(sheetFilePath))
        {
            EditorUtility.DisplayDialog("오류", "유효한 .sheet 파일을 선택해주세요.", "확인");
            return;
        }

        string outputPath = Path.ChangeExtension(sheetFilePath, ".osu");
        string saveFilePath = EditorUtility.SaveFilePanel("Osu 파일 저장", Path.GetDirectoryName(sheetFilePath), Path.GetFileNameWithoutExtension(sheetFilePath), "osu");

        if (!string.IsNullOrEmpty(saveFilePath))
        {
            try
            {
                SheetToOsuConverter.ConvertAndSave(sheetFilePath, saveFilePath, audioFileName, creator, version);
                EditorUtility.DisplayDialog("성공", $"변환 완료!\n{saveFilePath}", "확인");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("오류", $"변환 실패:\n{e.Message}", "확인");
                Debug.LogError(e);
            }
        }
    }

    private void ConvertOsuToSheet()
    {
        if (string.IsNullOrEmpty(osuFilePath) || !File.Exists(osuFilePath))
        {
            EditorUtility.DisplayDialog("오류", "유효한 .osu 파일을 선택해주세요.", "확인");
            return;
        }

        string saveFilePath = EditorUtility.SaveFilePanel("Sheet 파일 저장", Path.GetDirectoryName(osuFilePath), Path.GetFileNameWithoutExtension(osuFilePath), "sheet");

        if (!string.IsNullOrEmpty(saveFilePath))
        {
            try
            {
                OsuToSheetConverter.ConvertAndSave(osuFilePath, saveFilePath);
                EditorUtility.DisplayDialog("성공", $"변환 완료!\n{saveFilePath}", "확인");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("오류", $"변환 실패:\n{e.Message}", "확인");
                Debug.LogError(e);
            }
        }
    }

    private void TestConversion()
    {
        string testSheetPath = Path.Combine(Application.dataPath, "Sheet/Consolation/Consolation.sheet");

        if (!File.Exists(testSheetPath))
        {
            EditorUtility.DisplayDialog("오류", $"테스트 파일을 찾을 수 없습니다:\n{testSheetPath}", "확인");
            return;
        }

        try
        {
            // .sheet -> .osu 변환
            string osuPath = Path.Combine(Application.dataPath, "Sheet/Consolation/Consolation.osu");
            SheetToOsuConverter.ConvertAndSave(testSheetPath, osuPath, "Consolation.mp3", "Unity Rhythm Game", "4K");
            Debug.Log($"✓ Sheet -> Osu 변환 완료: {osuPath}");

            // .osu -> .sheet 변환 (다시 변환)
            string sheetPath2 = Path.Combine(Application.dataPath, "Sheet/Consolation/Consolation_converted.sheet");
            OsuToSheetConverter.ConvertAndSave(osuPath, sheetPath2);
            Debug.Log($"✓ Osu -> Sheet 변환 완료: {sheetPath2}");

            // 결과 비교
            string original = File.ReadAllText(testSheetPath);
            string converted = File.ReadAllText(sheetPath2);

            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log("원본 파일 (처음 20줄):");
            Debug.Log(string.Join("\n", original.Split('\n'), 0, System.Math.Min(20, original.Split('\n').Length)));
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log("변환된 파일 (처음 20줄):");
            Debug.Log(string.Join("\n", converted.Split('\n'), 0, System.Math.Min(20, converted.Split('\n').Length)));
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            EditorUtility.DisplayDialog("테스트 완료",
                $"변환 테스트가 완료되었습니다!\n\n" +
                $"생성된 파일:\n" +
                $"1. {osuPath}\n" +
                $"2. {sheetPath2}\n\n" +
                $"콘솔에서 결과를 확인하세요.",
                "확인");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("오류", $"테스트 실패:\n{e.Message}", "확인");
            Debug.LogError(e);
        }
    }
}
