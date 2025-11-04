# 빠른 시작 가이드 ⚡

osu!mania 4K 맵을 Unity 리듬 게임으로 가져오는 가장 빠른 방법입니다.

## 🚀 3단계로 시작하기

### 1️⃣ 단일 파일 변환

```bash
python3 osu2sheet.py "beatmap [4K].osu"
```

### 2️⃣ Unity로 복사

```bash
# 폴더 생성
mkdir -p Assets/Sheet/곡이름

# 파일 복사 (이름 통일 필수!)
cp "beatmap.sheet" Assets/Sheet/곡이름/곡이름.sheet
cp "audio.mp3" Assets/Sheet/곡이름/곡이름.mp3
cp "bg.jpg" Assets/Sheet/곡이름/곡이름.jpg
```

### 3️⃣ Unity에서 실행

Unity 에디터에서 게임 실행 → 곡 선택 화면에서 확인!

---

## 🔥 자동화 (권장)

모든 4K 맵을 자동으로 변환하고 Unity로 복사:

### Linux/macOS
```bash
./convert_and_import.sh
```

### Windows (PowerShell)
```powershell
.\convert_and_import.ps1
```

---

## 📚 더 자세한 내용

- **완벽한 가이드**: [USAGE.md](USAGE.md) - 초보자용 상세 가이드
- **README**: [README.md](README.md) - 프로젝트 소개

---

## ❓ 문제 해결

### Python이 없어요
```bash
# Linux
sudo apt install python3

# macOS
brew install python3

# Windows
# https://www.python.org/downloads/ 에서 다운로드
```

### Unity에서 곡이 안 보여요

**체크리스트:**
1. ✅ 폴더명과 파일명(확장자 제외)이 **모두 동일**한지 확인
2. ✅ `Assets/Sheet/곡이름/` 폴더 안에 있는지 확인
3. ✅ `.sheet`, `.mp3` 파일이 **반드시** 있어야 함
4. ✅ Unity 에디터 재시작

**올바른 구조:**
```
Assets/Sheet/
└── Consolation/
    ├── Consolation.sheet  ✅
    ├── Consolation.mp3    ✅
    └── Consolation.jpg    ✅ (선택사항)
```

### 4K 맵인지 어떻게 알아요?

파일명이나 osu에서 플레이 시 **4개 레인**이면 4K 맵입니다.

---

## 💡 팁

### 여러 파일 한번에 변환
```bash
python3 osu2sheet.py *[4K]*.osu
```

### 특정 난이도만
```bash
python3 osu2sheet.py *Easy*.osu
python3 osu2sheet.py *Hard*.osu
```

### osu 맵 위치

- **Windows**: `C:\Users\사용자명\AppData\Local\osu!\Songs\`
- **Linux**: `~/.local/share/osu/Songs/`
- **macOS**: `~/Library/Application Support/osu!/Songs/`

---

**더 자세한 내용은 [USAGE.md](USAGE.md)를 확인하세요!** 📖
