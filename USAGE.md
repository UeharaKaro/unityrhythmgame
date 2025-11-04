# osu2sheet 사용 가이드

osu!mania 4K 비트맵을 Unity 리듬 게임으로 가져오는 방법입니다.

## 빠른 시작

```bash
# 1. osu 파일을 sheet 파일로 변환
python3 osu2sheet.py beatmap.osu

# 2. 생성된 파일들을 Unity 프로젝트로 복사
# beatmap.sheet → Assets/Sheet/곡이름/곡이름.sheet
# beatmap.mp3 → Assets/Sheet/곡이름/곡이름.mp3
# 자켓.jpg → Assets/Sheet/곡이름/곡이름.jpg
```

## 단계별 가이드

### 1. osu 비트맵 준비

osu에서 4K 비트맵을 다운로드하거나 직접 제작합니다.

**비트맵 위치 (Windows):**
```
C:\Users\사용자명\AppData\Local\osu!\Songs\
```

**비트맵 위치 (Linux/macOS):**
```
~/.local/share/osu/Songs/
```

### 2. 변환 실행

```bash
# 단일 파일 변환
python3 osu2sheet.py "Songs/12345 Artist - Title/difficulty [4K].osu"

# 특정 폴더의 모든 4K 맵 변환
python3 osu2sheet.py "Songs/12345 Artist - Title"/*.osu
```

### 3. 파일 확인

변환이 완료되면 같은 폴더에 `.sheet` 파일이 생성됩니다:

```
Songs/12345 Artist - Title/
├── difficulty [4K].osu       # 원본
├── difficulty [4K].sheet     # 변환된 파일 ✨
├── audio.mp3                 # 음악 파일
└── bg.jpg                    # 배경 이미지
```

### 4. Unity 프로젝트로 복사

```bash
# 예시: Consolation 곡을 추가하는 경우

# 1. 폴더 생성
mkdir -p Assets/Sheet/Consolation/

# 2. 파일 복사 (파일명을 곡 이름으로 통일)
cp "difficulty [4K].sheet" Assets/Sheet/Consolation/Consolation.sheet
cp audio.mp3 Assets/Sheet/Consolation/Consolation.mp3
cp bg.jpg Assets/Sheet/Consolation/Consolation.jpg  # 선택사항
```

**중요:** 파일명이 모두 같아야 합니다!
```
Assets/Sheet/Consolation/
├── Consolation.sheet
├── Consolation.mp3
└── Consolation.jpg
```

### 5. Unity에서 확인

Unity 에디터에서 게임을 실행하면 새 곡이 목록에 표시됩니다.

## 일괄 변환 스크립트

여러 곡을 한번에 변환하고 Unity로 복사하는 예제:

### Bash 스크립트 (Linux/macOS)

```bash
#!/bin/bash
# convert_all.sh

# osu Songs 폴더
OSU_SONGS="$HOME/.local/share/osu/Songs"
# Unity 프로젝트 경로
UNITY_SHEETS="./Assets/Sheet"

# 모든 4K 맵 찾기
find "$OSU_SONGS" -name "*[4K]*.osu" | while read osu_file; do
    echo "변환 중: $osu_file"

    # .sheet 파일 생성
    python3 osu2sheet.py "$osu_file"

    # 폴더 및 파일 정보 추출
    dir=$(dirname "$osu_file")
    beatmap_folder=$(basename "$dir")

    # .sheet 파일 경로
    sheet_file="${osu_file%.osu}.sheet"

    # Unity 폴더 생성
    mkdir -p "$UNITY_SHEETS/$beatmap_folder"

    # 파일 복사
    cp "$sheet_file" "$UNITY_SHEETS/$beatmap_folder/"

    # 음악 파일 찾기 및 복사 (.mp3 또는 .ogg)
    find "$dir" -maxdepth 1 \( -name "*.mp3" -o -name "*.ogg" \) -print0 | head -z -n 1 | xargs -0 -I {} cp {} "$UNITY_SHEETS/$beatmap_folder/"

    # 이미지 찾기 및 복사
    find "$dir" -maxdepth 1 \( -name "*.jpg" -o -name "*.png" \) -print0 | head -z -n 1 | xargs -0 -I {} cp {} "$UNITY_SHEETS/$beatmap_folder/"
done

echo "✅ 모든 변환 완료!"
```

### PowerShell 스크립트 (Windows)

```powershell
# convert_all.ps1

$osuSongs = "$env:LOCALAPPDATA\osu!\Songs"
$unitySheets = ".\Assets\Sheet"

Get-ChildItem -Path $osuSongs -Recurse -Filter "*[4K]*.osu" | ForEach-Object {
    Write-Host "변환 중: $($_.FullName)"

    # .sheet 파일 생성
    python osu2sheet.py $_.FullName

    # 폴더명
    $beatmapFolder = $_.Directory.Name

    # Unity 폴더 생성
    $targetDir = Join-Path $unitySheets $beatmapFolder
    New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

    # .sheet 파일 복사
    $sheetFile = $_.FullName -replace '.osu$', '.sheet'
    Copy-Item $sheetFile -Destination $targetDir

    # 음악 파일 복사
    $audioFile = Get-ChildItem -Path $_.Directory -Filter "*.mp3" | Select-Object -First 1
    if ($audioFile) {
        Copy-Item $audioFile.FullName -Destination $targetDir
    }

    # 이미지 파일 복사
    $imageFile = Get-ChildItem -Path $_.Directory -Filter "*.jpg" | Select-Object -First 1
    if ($imageFile) {
        Copy-Item $imageFile.FullName -Destination $targetDir
    }
}

Write-Host "✅ 모든 변환 완료!"
```

## 파일명 통일하기

Unity에서 인식하려면 파일명이 통일되어야 합니다. 변환 후 이름 변경:

```bash
# 예시: "Consolation [4K].sheet" → "Consolation.sheet"
cd "Assets/Sheet/Consolation"
mv "Consolation [4K].sheet" "Consolation.sheet"
mv "audio.mp3" "Consolation.mp3"
mv "bg.jpg" "Consolation.jpg"
```

## 문제 해결

### 변환은 되는데 Unity에서 안 보이는 경우

1. 파일명 확인: `.sheet`, `.mp3`, `.jpg` 파일명이 모두 동일한지 확인
2. 폴더 구조 확인: `Assets/Sheet/곡이름/` 안에 있는지 확인
3. Unity 에디터 재시작

### CircleSize 경고

```
⚠️ 경고: 이 변환기는 4K 전용입니다. (현재 CircleSize: 7)
```

해당 비트맵은 7K 맵입니다. 4K 맵만 변환 가능합니다.

### 노트가 이상하게 배치된 경우

osu 맵이 4K가 아닌 경우 발생할 수 있습니다. 원본 맵이 정확히 4K인지 확인하세요.

## 유용한 팁

### 1. osu에서 4K 맵만 필터링

osu 게임 내에서:
- Mode: osu!mania
- Keys: 4K

### 2. 자동화 스크립트 실행

```bash
# 실행 권한 부여
chmod +x convert_all.sh

# 실행
./convert_all.sh
```

### 3. 특정 난이도만 변환

```bash
# Easy 난이도만
python3 osu2sheet.py Songs/*/*Easy*.osu

# Hard 난이도만
python3 osu2sheet.py Songs/*/*Hard*.osu
```

## 추가 정보

- Python 스크립트: `osu2sheet.py`
- Unity용 C# 변환기: `Assets/Scripts/OsuToSheetConverter.cs`
- 자세한 포맷 설명: `README.md`
