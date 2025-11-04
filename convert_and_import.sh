#!/bin/bash

# ===== 설정 =====
# 본인의 경로에 맞게 수정하세요!
OSU_SONGS="$HOME/.local/share/osu/Songs"  # osu Songs 폴더
UNITY_PROJECT="$(cd "$(dirname "$0")" && pwd)"  # 이 스크립트가 있는 폴더 (Unity 프로젝트 폴더)
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

# 경로 확인
echo -e "${YELLOW}설정 확인:${NC}"
echo "  osu Songs: $OSU_SONGS"
echo "  Unity 프로젝트: $UNITY_PROJECT"
echo "  변환 스크립트: $CONVERTER"
echo ""

# 변환기 확인
if [ ! -f "$CONVERTER" ]; then
    echo -e "${RED}❌ 변환 스크립트를 찾을 수 없습니다: $CONVERTER${NC}"
    exit 1
fi

# osu Songs 폴더 확인
if [ ! -d "$OSU_SONGS" ]; then
    echo -e "${RED}❌ osu Songs 폴더를 찾을 수 없습니다: $OSU_SONGS${NC}"
    echo -e "${YELLOW}💡 스크립트 상단의 OSU_SONGS 경로를 수정하세요.${NC}"
    exit 1
fi

# Unity Sheet 폴더 생성
mkdir -p "$UNITY_SHEETS"

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
success=0
failed=0

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
        ((success++))
    else
        echo -e "${RED}❌ 변환 실패${NC}"
        ((failed++))
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
    audio_file=$(find "$beatmap_dir" -maxdepth 1 \( -name "*.mp3" -o -name "*.ogg" \) 2>/dev/null | head -n 1)
    if [ -n "$audio_file" ]; then
        cp "$audio_file" "$unity_dir/"
        echo "  ✓ 음악 파일 복사: $(basename "$audio_file")"
    else
        echo -e "  ${YELLOW}⚠️  음악 파일 없음${NC}"
    fi

    # 이미지 파일 (.jpg 우선, 없으면 .png)
    image_file=$(find "$beatmap_dir" -maxdepth 1 \( -name "*.jpg" -o -name "*.png" \) 2>/dev/null | head -n 1)
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
echo -e "${GREEN}✅ 성공: ${success}개${NC}"
if [ $failed -gt 0 ]; then
    echo -e "${RED}❌ 실패: ${failed}개${NC}"
fi
echo ""
echo -e "${YELLOW}💡 Unity 에디터에서 게임을 실행하여 확인하세요!${NC}"
echo -e "${YELLOW}💡 폴더 위치: $UNITY_SHEETS${NC}"
