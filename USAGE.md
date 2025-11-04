# osu2sheet 완벽 사용 가이드

osu!mania 4K 비트맵을 Unity 리듬 게임으로 변환하는 완벽한 가이드입니다.

## 📋 목차

1. [시작하기 전에](#시작하기-전에)
2. [설치 및 준비](#설치-및-준비)
3. [기본 사용법](#기본-사용법)
4. [완전 초보자 튜토리얼](#완전-초보자-튜토리얼)
5. [일괄 변환 (여러 곡 한번에)](#일괄-변환)
6. [자동화 스크립트](#자동화-스크립트)
7. [문제 해결](#문제-해결)
8. [FAQ](#faq)

---

## 시작하기 전에

### 필요한 것

✅ **Python 3.6 이상** - [설치 방법](#python-설치)
✅ **osu!mania 4K 비트맵** - [다운로드 방법](#osu-비트맵-다운로드)
✅ **Unity 프로젝트** - 이미 받으셨다면 OK!

### Python 설치

#### Windows
1. https://www.python.org/downloads/ 접속
2. "Download Python" 버튼 클릭
3. 설치 시 **"Add Python to PATH"** 체크박스 반드시 선택!
4. 설치 완료 후 명령 프롬프트에서 확인:
   ```cmd
   python --version
   ```

#### macOS
```bash
# Homebrew로 설치 (권장)
brew install python3

# 확인
python3 --version
```

#### Linux (Ubuntu/Debian)
```bash
# 설치
sudo apt update
sudo apt install python3 python3-pip

# 확인
python3 --version
```

---

## 기본 사용법

### 1️⃣ 가장 간단한 방법

```bash
# osu2sheet.py가 있는 폴더로 이동
cd /path/to/unityrhythmgame

# 변환 실행
python3 osu2sheet.py 변환할파일.osu
```

**예시:**
```bash
python3 osu2sheet.py "Consolation [4K Normal].osu"
```

**결과:**
```
📂 입력: Consolation [4K Normal].osu
📄 출력: Consolation [4K Normal].sheet

🎵 곡명: Consolation
🎤 아티스트: Smilegate RPG
⏱️  BPM: 90
📍 Offset: 3245ms
🎼 박자: 3/4
🎹 노트 개수: 147

✅ 변환 완료!
```

### 2️⃣ 출력 파일명 지정

```bash
python3 osu2sheet.py input.osu -o output.sheet
```

**예시:**
```bash
python3 osu2sheet.py "beatmap.osu" -o "MyCustomName.sheet"
```

### 3️⃣ 여러 파일 한번에 변환

```bash
# 현재 폴더의 모든 .osu 파일
python3 osu2sheet.py *.osu

# 특정 패턴
python3 osu2sheet.py *[4K]*.osu
```

### 4️⃣ 도움말 보기

```bash
python3 osu2sheet.py --help
```

---

## 완전 초보자 튜토리얼

처음 사용하시는 분들을 위한 단계별 가이드입니다.

### 📦 Step 1: osu 비트맵 다운로드

#### 방법 A: osu 게임 내에서 다운로드

1. osu 게임 실행
2. 원하는 곡 검색
3. 다운로드 (4K 맵인지 확인!)
4. 다운로드된 위치:
   - **Windows:** `C:\Users\사용자명\AppData\Local\osu!\Songs\`
   - **macOS:** `~/Library/Application Support/osu!/Songs/`
   - **Linux:** `~/.local/share/osu/Songs/`

#### 방법 B: osu 웹사이트에서 다운로드

1. https://osu.ppy.sh/beatmapsets 접속
2. 검색창에서 원하는 곡 검색
3. Mode: **osu!mania** 필터 선택
4. 비트맵 클릭 → **Download** 버튼
5. 다운로드한 `.osz` 파일을 osu에서 열기

### 🔍 Step 2: 4K 맵 찾기

비트맵 폴더를 열면 이런 구조입니다:

```
Songs/
└── 123456 Artist - Song Name/
    ├── Artist - Song Name (Mapper) [Easy].osu
    ├── Artist - Song Name (Mapper) [Normal 4K].osu  ← 이거!
    ├── Artist - Song Name (Mapper) [Hard 7K].osu
    ├── audio.mp3
    └── bg.jpg
```

**중요:** 파일명이나 내용에 **"4K"** 표시가 있는 파일을 찾으세요!

### 🔄 Step 3: 변환 실행

#### Windows 사용자

1. **탐색기**에서 비트맵 폴더 열기
2. 주소창에 `cmd` 입력 → Enter (현재 폴더에서 명령 프롬프트 열림)
3. 다음 명령어 입력:
   ```cmd
   cd C:\path\to\unityrhythmgame
   python osu2sheet.py "C:\Users\사용자명\AppData\Local\osu!\Songs\123456 Artist - Song\beatmap [4K].osu"
   ```

#### macOS/Linux 사용자

1. **터미널** 열기
2. Unity 프로젝트 폴더로 이동:
   ```bash
   cd ~/Projects/unityrhythmgame
   ```
3. 변환 실행:
   ```bash
   python3 osu2sheet.py ~/.local/share/osu/Songs/123456\ Artist\ -\ Song/beatmap\ [4K].osu
   ```

💡 **팁:** 파일을 터미널로 드래그하면 경로가 자동으로 입력됩니다!

### 📂 Step 4: Unity 프로젝트에 추가

변환이 완료되면 3개 파일이 필요합니다:

```
✅ .sheet 파일 (방금 생성됨)
✅ .mp3 파일 (음악)
✅ .jpg 파일 (자켓 이미지, 선택사항)
```

#### 실제 예시: "Consolation" 곡 추가하기

**1단계: 폴더 생성**

```bash
# Unity 프로젝트 폴더에서
cd Assets/Sheet
mkdir Consolation
```

**2단계: 파일 복사 및 이름 변경**

```bash
# .sheet 파일
cp "원본경로/Consolation [4K].sheet" Consolation/Consolation.sheet

# 음악 파일
cp "원본경로/audio.mp3" Consolation/Consolation.mp3

# 자켓 이미지 (선택사항)
cp "원본경로/bg.jpg" Consolation/Consolation.jpg
```

**결과 구조:**

```
Assets/Sheet/Consolation/
├── Consolation.sheet  ✅
├── Consolation.mp3    ✅
└── Consolation.jpg    ✅
```

**⚠️ 중요:** 폴더명과 파일명(확장자 제외)이 **모두 동일**해야 합니다!

#### Windows 그래픽 방식

1. `Assets/Sheet/` 폴더 열기
2. 새 폴더 생성 → 이름: `Consolation`
3. 비트맵 폴더에서 3개 파일 복사:
   - `Consolation [4K].sheet` → `Consolation.sheet`로 이름 변경
   - `audio.mp3` → `Consolation.mp3`로 이름 변경
   - `bg.jpg` → `Consolation.jpg`로 이름 변경
4. `Consolation` 폴더에 붙여넣기

### 🎮 Step 5: Unity에서 확인

1. Unity 에디터 열기
2. 프로젝트 실행 (▶️ 버튼)
3. 곡 선택 화면에서 새로운 곡 확인!

---

## 일괄 변환

여러 곡을 한번에 변환하는 방법입니다.

### 방법 1: 와일드카드 사용

```bash
# 특정 폴더의 모든 4K 맵 변환
cd ~/.local/share/osu/Songs
python3 ~/Projects/unityrhythmgame/osu2sheet.py */*[4K]*.osu

# 특정 아티스트의 모든 맵
python3 osu2sheet.py */ARTIST*/*.osu

# Easy 난이도만
python3 osu2sheet.py */*Easy*.osu
```

### 방법 2: find 명령어 (Linux/macOS)

```bash
# osu Songs 폴더에서 모든 4K 맵 찾아서 변환
find ~/.local/share/osu/Songs -name "*[4K]*.osu" -exec python3 ~/Projects/unityrhythmgame/osu2sheet.py {} \;
```

### 방법 3: 폴더별로 하나씩

```bash
cd ~/.local/share/osu/Songs

# 각 곡 폴더별로
python3 ~/Projects/unityrhythmgame/osu2sheet.py "123456 Artist - Song1"/*.osu
python3 ~/Projects/unityrhythmgame/osu2sheet.py "234567 Artist - Song2"/*.osu
python3 ~/Projects/unityrhythmgame/osu2sheet.py "345678 Artist - Song3"/*.osu
```

---

## 자동화 스크립트

변환 + Unity 복사까지 자동으로 하는 스크립트입니다.

### 🐧 Linux/macOS: convert_and_import.sh

아래 내용을 `convert_and_import.sh` 파일로 저장:

```bash
#!/bin/bash

# ===== 설정 =====
# 본인의 경로에 맞게 수정하세요!
OSU_SONGS="$HOME/.local/share/osu/Songs"  # osu Songs 폴더
UNITY_PROJECT="$HOME/Projects/unityrhythmgame"  # Unity 프로젝트 폴더
UNITY_SHEETS="$UNITY_PROJECT/Assets/Sheet"  # Unity Sheet 폴더
CONVERTER="$UNITY_PROJECT/osu2sheet.py"  # 변환 스크립트

# ===== 색상 정의 =====
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  osu → Unity 자동 변환 및 임포트      ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════╝${NC}"
echo ""

# 4K 맵 찾기
echo -e "${YELLOW}🔍 4K 맵 검색 중...${NC}"
maplist=($(find "$OSU_SONGS" -name "*[4K]*.osu" 2>/dev/null))
total=${#maplist[@]}

if [ $total -eq 0 ]; then
    echo -e "${RED}❌ 4K 맵을 찾을 수 없습니다.${NC}"
    echo "osu Songs 폴더 경로를 확인하세요: $OSU_SONGS"
    exit 1
fi

echo -e "${GREEN}✅ ${total}개의 4K 맵을 찾았습니다!${NC}"
echo ""

# 변환 시작
count=0
for osu_file in "${maplist[@]}"; do
    ((count++))

    echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${BLUE}[$count/$total] 변환 중...${NC}"
    echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

    # 파일 정보
    beatmap_dir=$(dirname "$osu_file")
    beatmap_folder=$(basename "$beatmap_dir")
    osu_filename=$(basename "$osu_file")
    sheet_file="${osu_file%.osu}.sheet"

    echo "📁 폴더: $beatmap_folder"
    echo "📄 파일: $osu_filename"

    # 변환
    if python3 "$CONVERTER" "$osu_file" 2>&1 | grep -q "✅ 변환 완료"; then
        echo -e "${GREEN}✅ 변환 성공${NC}"
    else
        echo -e "${RED}❌ 변환 실패${NC}"
        continue
    fi

    # Unity 폴더 생성
    unity_dir="$UNITY_SHEETS/$beatmap_folder"
    mkdir -p "$unity_dir"

    # 파일 복사
    echo "📦 Unity로 복사 중..."

    # .sheet 파일
    if [ -f "$sheet_file" ]; then
        cp "$sheet_file" "$unity_dir/"
        echo "  ✓ .sheet 파일 복사"
    fi

    # 음악 파일 (.mp3 우선, 없으면 .ogg)
    audio_file=$(find "$beatmap_dir" -maxdepth 1 -name "*.mp3" -o -name "*.ogg" | head -n 1)
    if [ -n "$audio_file" ]; then
        cp "$audio_file" "$unity_dir/"
        echo "  ✓ 음악 파일 복사: $(basename "$audio_file")"
    else
        echo -e "  ${YELLOW}⚠️  음악 파일 없음${NC}"
    fi

    # 이미지 파일 (.jpg 우선, 없으면 .png)
    image_file=$(find "$beatmap_dir" -maxdepth 1 -name "*.jpg" -o -name "*.png" | head -n 1)
    if [ -n "$image_file" ]; then
        cp "$image_file" "$unity_dir/"
        echo "  ✓ 이미지 파일 복사: $(basename "$image_file")"
    else
        echo -e "  ${YELLOW}⚠️  이미지 파일 없음${NC}"
    fi

    echo -e "${GREEN}✅ 완료: $beatmap_folder${NC}"
    echo ""
done

echo -e "${BLUE}╔════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  모든 변환 완료!                      ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════╝${NC}"
echo ""
echo -e "${GREEN}✅ ${count}개의 곡을 Unity 프로젝트로 추가했습니다.${NC}"
echo -e "${YELLOW}💡 Unity 에디터에서 게임을 실행하여 확인하세요!${NC}"
```

**사용 방법:**

```bash
# 1. 실행 권한 부여
chmod +x convert_and_import.sh

# 2. 스크립트 실행
./convert_and_import.sh
```

### 🪟 Windows: convert_and_import.ps1

아래 내용을 `convert_and_import.ps1` 파일로 저장:

```powershell
# ===== 설정 =====
# 본인의 경로에 맞게 수정하세요!
$osuSongs = "$env:LOCALAPPDATA\osu!\Songs"
$unityProject = "C:\Projects\unityrhythmgame"
$unitySheets = "$unityProject\Assets\Sheet"
$converter = "$unityProject\osu2sheet.py"

# ===== 시작 =====
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║  osu → Unity 자동 변환 및 임포트      ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""

# 4K 맵 찾기
Write-Host "🔍 4K 맵 검색 중..." -ForegroundColor Yellow
$maplist = Get-ChildItem -Path $osuSongs -Recurse -Filter "*[4K]*.osu" -ErrorAction SilentlyContinue
$total = $maplist.Count

if ($total -eq 0) {
    Write-Host "❌ 4K 맵을 찾을 수 없습니다." -ForegroundColor Red
    Write-Host "osu Songs 폴더 경로를 확인하세요: $osuSongs"
    exit 1
}

Write-Host "✅ ${total}개의 4K 맵을 찾았습니다!" -ForegroundColor Green
Write-Host ""

# 변환 시작
$count = 0
foreach ($osuFile in $maplist) {
    $count++

    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Blue
    Write-Host "[$count/$total] 변환 중..." -ForegroundColor Blue
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Blue

    # 파일 정보
    $beatmapFolder = $osuFile.Directory.Name
    $osuFilename = $osuFile.Name

    Write-Host "📁 폴더: $beatmapFolder"
    Write-Host "📄 파일: $osuFilename"

    # 변환
    $output = & python $converter $osuFile.FullName 2>&1 | Out-String
    if ($output -match "✅ 변환 완료") {
        Write-Host "✅ 변환 성공" -ForegroundColor Green
    } else {
        Write-Host "❌ 변환 실패" -ForegroundColor Red
        continue
    }

    # Unity 폴더 생성
    $unityDir = Join-Path $unitySheets $beatmapFolder
    New-Item -ItemType Directory -Force -Path $unityDir | Out-Null

    # 파일 복사
    Write-Host "📦 Unity로 복사 중..."

    # .sheet 파일
    $sheetFile = $osuFile.FullName -replace '\.osu$', '.sheet'
    if (Test-Path $sheetFile) {
        Copy-Item $sheetFile -Destination $unityDir -Force
        Write-Host "  ✓ .sheet 파일 복사"
    }

    # 음악 파일
    $audioFile = Get-ChildItem -Path $osuFile.Directory -Filter "*.mp3" -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $audioFile) {
        $audioFile = Get-ChildItem -Path $osuFile.Directory -Filter "*.ogg" -ErrorAction SilentlyContinue | Select-Object -First 1
    }
    if ($audioFile) {
        Copy-Item $audioFile.FullName -Destination $unityDir -Force
        Write-Host "  ✓ 음악 파일 복사: $($audioFile.Name)"
    } else {
        Write-Host "  ⚠️  음악 파일 없음" -ForegroundColor Yellow
    }

    # 이미지 파일
    $imageFile = Get-ChildItem -Path $osuFile.Directory -Filter "*.jpg" -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $imageFile) {
        $imageFile = Get-ChildItem -Path $osuFile.Directory -Filter "*.png" -ErrorAction SilentlyContinue | Select-Object -First 1
    }
    if ($imageFile) {
        Copy-Item $imageFile.FullName -Destination $unityDir -Force
        Write-Host "  ✓ 이미지 파일 복사: $($imageFile.Name)"
    } else {
        Write-Host "  ⚠️  이미지 파일 없음" -ForegroundColor Yellow
    }

    Write-Host "✅ 완료: $beatmapFolder" -ForegroundColor Green
    Write-Host ""
}

Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║  모든 변환 완료!                      ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""
Write-Host "✅ ${count}개의 곡을 Unity 프로젝트로 추가했습니다." -ForegroundColor Green
Write-Host "💡 Unity 에디터에서 게임을 실행하여 확인하세요!" -ForegroundColor Yellow
```

**사용 방법:**

```powershell
# PowerShell에서 실행
.\convert_and_import.ps1
```

---

## 문제 해결

### ❌ "python을 찾을 수 없습니다" 에러

**원인:** Python이 설치되지 않았거나 PATH에 등록되지 않음

**해결:**
1. Python 설치 확인:
   ```bash
   python --version
   # 또는
   python3 --version
   ```
2. 안 되면 Python 재설치 (PATH 옵션 체크!)

### ❌ "파일을 찾을 수 없습니다" 에러

**원인:** 파일 경로가 잘못됨

**해결:**
```bash
# 파일 경로에 공백이 있으면 따옴표로 감싸기
python3 osu2sheet.py "파일 이름 with spaces.osu"

# 절대 경로 사용
python3 osu2sheet.py "/full/path/to/beatmap.osu"
```

### ⚠️ "CircleSize가 4가 아닙니다" 경고

**원인:** 해당 맵이 4K가 아님 (5K, 7K 등)

**해결:** 4K 맵만 선택해서 변환하세요

```bash
# 파일명에 4K가 있는 것만 변환
python3 osu2sheet.py *[4K]*.osu
```

### ❌ Unity에서 곡이 안 보임

**체크리스트:**

1. **파일명이 모두 동일한가요?**
   ```
   ✅ Consolation/Consolation.sheet
   ✅ Consolation/Consolation.mp3
   ✅ Consolation/Consolation.jpg

   ❌ Consolation/song.sheet  (다른 이름)
   ❌ Consolation/audio.mp3   (다른 이름)
   ```

2. **폴더 구조가 맞나요?**
   ```
   ✅ Assets/Sheet/Consolation/Consolation.sheet
   ❌ Assets/Consolation/Consolation.sheet  (Sheet 폴더 없음)
   ❌ Assets/Sheet/Consolation.sheet  (하위 폴더 없음)
   ```

3. **파일이 3개 모두 있나요?**
   - `.sheet` 파일 (필수)
   - `.mp3` 파일 (필수)
   - `.jpg` 파일 (선택사항)

4. **Unity 에디터 재시작**
   - 파일 추가 후 Unity 에디터 완전히 종료 후 재실행

### ❌ 노트가 이상하게 나옴

**원인:** 4K가 아닌 맵을 변환했거나 맵 자체가 잘못됨

**해결:**
1. osu에서 해당 맵을 직접 플레이해서 정상인지 확인
2. 4K 맵이 맞는지 다시 확인
3. 다른 난이도로 시도

### 🐌 변환이 너무 느림

**원인:** 수백 개의 파일을 한번에 변환하는 경우

**해결:**
```bash
# 병렬 처리 (Linux/macOS)
find Songs -name "*[4K]*.osu" | xargs -P 4 -I {} python3 osu2sheet.py {}
# -P 4 = 4개 동시 처리
```

---

## FAQ

### Q1: .sheet 파일이 정확히 뭔가요?

A: Unity 리듬 게임에서 사용하는 맵 파일 형식입니다. 노트의 타이밍, 위치, 타입 등을 담고 있습니다.

```
[Description]
Title: 곡 제목
Artist: 아티스트

[Audio]
BPM: 90
Offset: 3245
Signature: 3/4

[Note]
3245, 0, 1              # 3245ms, 단노트, 레인1
5620, 1, 4, 7245        # 5620~7245ms, 롱노트, 레인4
```

### Q2: 5K, 6K, 7K 맵도 변환할 수 있나요?

A: 아니요, 현재는 4K 맵만 지원합니다. 이 Unity 리듬 게임이 4레인 전용이기 때문입니다.

### Q3: .mp3 대신 .ogg 파일도 되나요?

A: Unity에서는 `.mp3`, `.ogg`, `.wav` 모두 지원하지만, 일관성을 위해 `.mp3`로 변환하는 것을 권장합니다.

```bash
# ffmpeg로 변환
ffmpeg -i audio.ogg -codec:a libmp3lame -qscale:a 2 audio.mp3
```

### Q4: 자켓 이미지가 꼭 필요한가요?

A: 필수는 아니지만, 있으면 곡 선택 화면에서 더 예쁘게 보입니다.

### Q5: BPM 변화가 있는 곡은?

A: 현재 버전은 단일 BPM만 지원합니다. 첫 번째 타이밍 포인트의 BPM을 사용합니다.

### Q6: 변환한 .sheet 파일을 다시 .osu로 바꿀 수 있나요?

A: 현재 이 도구는 osu → sheet 단방향만 지원합니다.

### Q7: 다른 리듬 게임 형식도 지원하나요?

A: 현재는 osu!mania 4K만 지원합니다.

### Q8: 상업적으로 사용해도 되나요?

A: 변환 도구는 자유롭게 사용 가능하지만, osu 비트맵의 저작권은 각 맵 제작자에게 있으므로 주의하세요.

### Q9: 변환 속도를 높이려면?

A: 병렬 처리를 사용하세요:

```bash
# Linux/macOS (4개 동시 처리)
find Songs -name "*.osu" | xargs -P 4 -I {} python3 osu2sheet.py {}

# Windows (PowerShell, 병렬 처리)
Get-ChildItem *.osu | ForEach-Object -Parallel { python osu2sheet.py $_.FullName } -ThrottleLimit 4
```

### Q10: 에러가 계속 발생해요

A: 다음을 확인하세요:

1. Python 버전: `python3 --version` (3.6 이상)
2. 파일 경로: 공백이 있으면 따옴표 사용
3. 파일 권한: 읽기/쓰기 권한 확인
4. 디스크 공간: 충분한 저장 공간 확인

그래도 안 되면 에러 메시지를 복사해서 이슈로 등록해주세요.

---

## 추가 도움말

### 유용한 명령어 모음

```bash
# 변환된 .sheet 파일 개수 확인
ls -1 *.sheet | wc -l

# 특정 아티스트 맵만 변환
python3 osu2sheet.py */IOSYS*/*.osu

# Easy 난이도만
python3 osu2sheet.py */*Easy*.osu

# Hard 이상만
python3 osu2sheet.py */*Hard*.osu */*Insane*.osu

# 최근 다운로드한 맵 (최근 7일)
find Songs -name "*.osu" -mtime -7 -exec python3 osu2sheet.py {} \;
```

### 디버깅 팁

```bash
# 상세 출력 (에러 확인)
python3 osu2sheet.py beatmap.osu 2>&1 | tee convert.log

# 변환 전 맵 정보만 확인 (실제 변환 안 함)
head -n 50 beatmap.osu  # 처음 50줄 보기
grep "CircleSize" beatmap.osu  # 4K인지 확인
```

---

## 관련 링크

- **osu 공식 사이트:** https://osu.ppy.sh/
- **osu 비트맵 검색:** https://osu.ppy.sh/beatmapsets
- **Unity 공식 사이트:** https://unity.com/
- **Python 다운로드:** https://www.python.org/downloads/

---

**도움이 필요하시면 이슈를 등록해주세요!** 🙏
