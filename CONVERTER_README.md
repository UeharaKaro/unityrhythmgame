# Sheet ↔ Osu 파일 변환기

Unity 리듬 게임의 `.sheet` 파일과 osu!mania 4K의 `.osu` 파일을 서로 변환하는 도구입니다.

## 기능

- ✅ `.sheet` → `.osu` 변환 (Unity 리듬 게임 맵을 osu!mania로)
- ✅ `.osu` → `.sheet` 변환 (osu!mania 맵을 Unity 리듬 게임으로)
- ✅ 4개 레인 (4K) 지원
- ✅ 단노트 (Short Note) 지원
- ✅ 롱노트 (Long Note/Hold Note) 지원
- ✅ 완벽한 타이밍 일치 (밀리초 단위)

## 파일 위치

변환기 관련 파일들:

```
Assets/Scripts/
├── OsuToSheetConverter.cs        # .osu → .sheet 변환기
├── SheetToOsuConverter.cs        # .sheet → .osu 변환기
└── Editor/
    └── SheetOsuConverterEditor.cs # Unity 에디터 GUI
```

## Unity 에디터에서 사용하기

### 1. 에디터 창 열기

Unity 에디터에서:
1. 상단 메뉴 `Tools` → `Sheet <-> Osu Converter` 클릭
2. 변환기 창이 열립니다

### 2. Sheet → Osu 변환

1. "Sheet 파일" 필드에서 `찾기` 버튼을 클릭하여 `.sheet` 파일 선택
2. 필요한 정보 입력:
   - **오디오 파일명**: osu에서 사용할 음악 파일 이름 (예: `audio.mp3`)
   - **제작자**: 맵 제작자 이름
   - **난이도명**: 난이도 이름 (예: `4K`, `Normal`, `Hard`)
3. `Sheet → Osu 변환` 버튼 클릭
4. 저장 위치 선택

### 3. Osu → Sheet 변환

1. "Osu 파일" 필드에서 `찾기` 버튼을 클릭하여 `.osu` 파일 선택
2. `Osu → Sheet 변환` 버튼 클릭
3. 저장 위치 선택

### 4. 빠른 테스트

`기존 Sheet 파일 테스트 (Consolation)` 버튼을 클릭하면:
1. `Consolation.sheet` → `Consolation.osu` 변환
2. `Consolation.osu` → `Consolation_converted.sheet` 재변환
3. 결과를 콘솔에 출력

## 코드에서 사용하기

### Sheet → Osu 변환

```csharp
using System.IO;

// 방법 1: 파일 경로로 직접 변환
string sheetPath = "path/to/song.sheet";
string osuPath = "path/to/song.osu";
SheetToOsuConverter.ConvertAndSave(
    sheetPath,
    osuPath,
    audioFileName: "song.mp3",
    creator: "MyName",
    version: "4K Hard"
);

// 방법 2: Sheet 객체를 osu 문자열로 변환
Sheet sheet = /* ... */;
string osuContent = SheetToOsuConverter.ConvertSheetToOsu(
    sheet,
    audioFileName: "song.mp3",
    creator: "MyName",
    version: "4K"
);
File.WriteAllText("output.osu", osuContent);
```

### Osu → Sheet 변환

```csharp
// 방법 1: 파일 경로로 직접 변환
string osuPath = "path/to/beatmap.osu";
string sheetPath = "path/to/output.sheet";
OsuToSheetConverter.ConvertAndSave(osuPath, sheetPath);

// 방법 2: osu 파일을 Sheet 객체로 변환
Sheet sheet = OsuToSheetConverter.ConvertOsuToSheet("path/to/beatmap.osu");
// sheet 객체 사용...
```

## 파일 포맷 상세

### .sheet 파일 구조

```
[Description]
Title: 곡 제목
Artist: 아티스트 이름

[Audio]
BPM: 90
Offset: 3245          # 밀리초 단위
Signature: 3/4        # 박자

[Note]
time, type, line [, tail]
```

- **time**: 노트 시간 (밀리초)
- **type**: `0` = 단노트, `1` = 롱노트
- **line**: 레인 번호 (`1`~`4`)
- **tail**: 롱노트의 끝 시간 (롱노트인 경우에만)

예시:
```
3245, 0, 1          # 3245ms, 단노트, 레인1
5620, 1, 4, 7245    # 5620~7245ms, 롱노트, 레인4
```

### .osu 파일 구조 (osu!mania 4K)

```
[General]
Mode: 3             # 3 = osu!mania

[Difficulty]
CircleSize: 4       # 4 = 4K

[TimingPoints]
offset,msPerBeat,meter,sampleSet,sampleIndex,volume,uninherited,effects

[HitObjects]
x,y,time,type,hitSound,endTime:hitSample
```

**HitObjects 형식:**
- **x**: 레인 위치 (`64`, `192`, `320`, `448`)
- **y**: 192 (고정, mania에서 무시됨)
- **time**: 노트 시간 (밀리초)
- **type**: `1` = 일반 노트, `128` = 롱노트
- **endTime**: 롱노트의 끝 시간

예시:
```
64,192,3245,1,0,0:0:0:0:           # 3245ms, 단노트, 레인1
448,192,5620,128,0,7245:0:0:0:0:   # 5620~7245ms, 롱노트, 레인4
```

## 레인 매핑

| .sheet (line) | .osu (x 좌표) | 범위 |
|--------------|--------------|------|
| 1 | 64 | 0-127 |
| 2 | 192 | 128-255 |
| 3 | 320 | 256-383 |
| 4 | 448 | 384-511 |

## 변환 정확도

- ✅ **타이밍**: 밀리초 단위로 완벽하게 일치
- ✅ **BPM**: 정확하게 변환
- ✅ **Offset**: 그대로 유지
- ✅ **노트 타입**: 단노트/롱노트 구분 유지
- ✅ **레인 위치**: 4K 레인 정확하게 매핑
- ✅ **롱노트 길이**: 시작/끝 시간 정확하게 유지

`.osu` → `.sheet` → `.osu` 변환 후에도 노트 배치가 완벽하게 동일합니다.

## 주의사항

1. **4K 전용**: 이 변환기는 4개 레인(4K) 모드만 지원합니다.
2. **osu!mania 전용**: osu!standard, taiko, catch는 지원하지 않습니다.
3. **타이밍 포인트**: 현재 단일 BPM만 지원합니다. (BPM 변화는 향후 추가 예정)
4. **오디오 파일**: 변환 시 음악 파일은 별도로 복사해야 합니다.

## 예제

### 예제 1: 기존 맵을 osu로 내보내기

```csharp
// Consolation 맵을 osu로 변환
SheetToOsuConverter.ConvertAndSave(
    "Assets/Sheet/Consolation/Consolation.sheet",
    "Assets/Sheet/Consolation/Consolation.osu",
    audioFileName: "Consolation.mp3",
    creator: "Unity Rhythm Game",
    version: "4K Normal"
);
```

### 예제 2: osu 맵을 가져오기

```csharp
// 다운로드한 osu 맵을 Sheet 형식으로 변환
OsuToSheetConverter.ConvertAndSave(
    "Downloads/beatmap.osu",
    "Assets/Sheet/NewSong/NewSong.sheet"
);
// 주의: 음악 파일과 이미지는 수동으로 복사 필요
```

### 예제 3: 양방향 변환 테스트

```csharp
// 원본 파일
string original = "Assets/Sheet/Consolation/Consolation.sheet";

// Sheet → Osu
string osuFile = "temp.osu";
SheetToOsuConverter.ConvertAndSave(original, osuFile);

// Osu → Sheet
string reconverted = "reconverted.sheet";
OsuToSheetConverter.ConvertAndSave(osuFile, reconverted);

// 결과: reconverted.sheet의 노트 데이터가 original과 동일
```

## 문제 해결

### "CircleSize가 4가 아닙니다" 경고

- osu 맵이 4K가 아닌 경우 발생합니다.
- 5K, 6K, 7K 등의 맵은 현재 지원하지 않습니다.

### 변환 후 노트가 안 맞음

1. 원본 파일의 BPM과 Offset이 올바른지 확인
2. osu 파일의 TimingPoints가 올바른지 확인
3. 콘솔 로그에서 경고 메시지 확인

### Unity 에디터에서 메뉴가 안 보임

1. `Assets/Scripts/Editor` 폴더에 파일이 있는지 확인
2. Unity 에디터 재시작
3. 컴파일 에러가 없는지 확인

## 라이선스

이 프로젝트의 라이선스를 따릅니다.

## 기여

버그 리포트나 기능 제안은 이슈로 등록해주세요!
