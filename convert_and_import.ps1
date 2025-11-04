# ===== 설정 =====
# 본인의 경로에 맞게 수정하세요!
$osuSongs = "$env:LOCALAPPDATA\osu!\Songs"
$unityProject = Split-Path -Parent $MyInvocation.MyCommand.Path  # 이 스크립트가 있는 폴더
$unitySheets = "$unityProject\Assets\Sheet"
$converter = "$unityProject\osu2sheet.py"

# ===== 시작 =====
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║  osu → Unity 자동 변환 및 임포트      ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""

# 경로 확인
Write-Host "설정 확인:" -ForegroundColor Yellow
Write-Host "  osu Songs: $osuSongs"
Write-Host "  Unity 프로젝트: $unityProject"
Write-Host "  변환 스크립트: $converter"
Write-Host ""

# 변환기 확인
if (-not (Test-Path $converter)) {
    Write-Host "❌ 변환 스크립트를 찾을 수 없습니다: $converter" -ForegroundColor Red
    exit 1
}

# osu Songs 폴더 확인
if (-not (Test-Path $osuSongs)) {
    Write-Host "❌ osu Songs 폴더를 찾을 수 없습니다: $osuSongs" -ForegroundColor Red
    Write-Host "💡 스크립트 상단의 `$osuSongs 경로를 수정하세요." -ForegroundColor Yellow
    exit 1
}

# Unity Sheet 폴더 생성
New-Item -ItemType Directory -Force -Path $unitySheets | Out-Null

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
$success = 0
$failed = 0

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
    try {
        $output = & python $converter $osuFile.FullName 2>&1 | Out-String
        if ($output -match "✅ 변환 완료") {
            Write-Host "✅ 변환 성공" -ForegroundColor Green
            $success++
        } else {
            Write-Host "❌ 변환 실패" -ForegroundColor Red
            $failed++
            continue
        }
    } catch {
        Write-Host "❌ 변환 중 에러 발생" -ForegroundColor Red
        $failed++
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
Write-Host "✅ 성공: ${success}개" -ForegroundColor Green
if ($failed -gt 0) {
    Write-Host "❌ 실패: ${failed}개" -ForegroundColor Red
}
Write-Host ""
Write-Host "💡 Unity 에디터에서 게임을 실행하여 확인하세요!" -ForegroundColor Yellow
Write-Host "💡 폴더 위치: $unitySheets" -ForegroundColor Yellow
