#!/usr/bin/env python3
"""
osu!mania 4K .osu 파일을 Unity 리듬 게임 .sheet 파일로 변환하는 CLI 도구
"""

import argparse
import sys
import os
from pathlib import Path


class OsuToSheetConverter:
    """osu 파일을 sheet 파일로 변환하는 클래스"""

    MANIA_KEY_COUNT = 4

    def __init__(self):
        self.title = ""
        self.artist = ""
        self.bpm = 120
        self.offset = 0
        self.signature = [4, 4]
        self.notes = []

    def parse_osu_file(self, osu_path):
        """osu 파일을 파싱"""
        if not os.path.exists(osu_path):
            raise FileNotFoundError(f"파일을 찾을 수 없습니다: {osu_path}")

        current_section = ""

        with open(osu_path, 'r', encoding='utf-8') as f:
            for line in f:
                line = line.strip()

                # 섹션 확인
                if line.startswith('[') and line.endswith(']'):
                    current_section = line
                    continue

                # 빈 줄이나 주석 무시
                if not line or line.startswith('//'):
                    continue

                # Metadata 섹션
                if current_section == '[Metadata]':
                    if line.startswith('Title:'):
                        self.title = line[6:].strip()
                    elif line.startswith('Artist:'):
                        self.artist = line[7:].strip()

                # Difficulty 섹션
                elif current_section == '[Difficulty]':
                    if line.startswith('CircleSize:'):
                        circle_size = int(line[11:].strip())
                        if circle_size != self.MANIA_KEY_COUNT:
                            print(f"⚠️  경고: 이 변환기는 4K 전용입니다. (현재 CircleSize: {circle_size})")

                # TimingPoints 섹션
                elif current_section == '[TimingPoints]':
                    parts = line.split(',')
                    if len(parts) >= 2:
                        # uninherited (빨간선) 타이밍 포인트만 처리
                        is_uninherited = len(parts) < 7 or parts[6].strip() == '1'
                        if is_uninherited and self.offset == 0:
                            self.offset = int(round(float(parts[0].strip())))
                            ms_per_beat = float(parts[1].strip())
                            self.bpm = int(round(60000.0 / ms_per_beat))

                            if len(parts) >= 3:
                                meter = int(parts[2].strip())
                                self.signature = [meter, 4]
                            else:
                                self.signature = [4, 4]

                # HitObjects 섹션
                elif current_section == '[HitObjects]':
                    parts = line.split(',')
                    if len(parts) >= 4:
                        x = int(parts[0].strip())
                        time = int(parts[2].strip())
                        obj_type = int(parts[3].strip())

                        # x 좌표를 레인 번호로 변환 (4K 기준)
                        lane = (x * self.MANIA_KEY_COUNT // 512) + 1
                        if lane > self.MANIA_KEY_COUNT:
                            lane = self.MANIA_KEY_COUNT
                        if lane < 1:
                            lane = 1

                        # type & 128 = 롱노트
                        if obj_type & 128:  # 롱노트
                            if len(parts) >= 6:
                                end_params = parts[5].split(':')
                                end_time = int(end_params[0].strip())
                                self.notes.append({
                                    'time': time,
                                    'type': 1,  # Long
                                    'line': lane,
                                    'tail': end_time
                                })
                        else:  # 일반 노트
                            self.notes.append({
                                'time': time,
                                'type': 0,  # Short
                                'line': lane,
                                'tail': -1
                            })

        # 노트를 시간 순으로 정렬
        self.notes.sort(key=lambda x: x['time'])

    def write_sheet_file(self, sheet_path):
        """sheet 파일로 저장"""
        with open(sheet_path, 'w', encoding='utf-8') as f:
            # Description 섹션
            f.write('[Description]\n')
            f.write(f'Title: {self.title}\n')
            f.write(f'Artist: {self.artist}\n')
            f.write('\n')

            # Audio 섹션
            f.write('[Audio]\n')
            f.write(f'BPM: {self.bpm}\n')
            f.write(f'Offset: {self.offset}\n')
            f.write(f'Signature: {self.signature[0]}/{self.signature[1]}\n')
            f.write('\n')

            # Note 섹션
            f.write('[Note]\n')
            for note in self.notes:
                if note['type'] == 0:  # Short
                    f.write(f"{note['time']}, {note['type']}, {note['line']}\n")
                else:  # Long
                    f.write(f"{note['time']}, {note['type']}, {note['line']}, {note['tail']}\n")

    def convert(self, osu_path, sheet_path):
        """변환 실행"""
        print(f"📂 입력: {osu_path}")
        print(f"📄 출력: {sheet_path}")
        print()

        self.parse_osu_file(osu_path)

        print(f"🎵 곡명: {self.title}")
        print(f"🎤 아티스트: {self.artist}")
        print(f"⏱️  BPM: {self.bpm}")
        print(f"📍 Offset: {self.offset}ms")
        print(f"🎼 박자: {self.signature[0]}/{self.signature[1]}")
        print(f"🎹 노트 개수: {len(self.notes)}")
        print()

        self.write_sheet_file(sheet_path)

        print("✅ 변환 완료!")


def main():
    parser = argparse.ArgumentParser(
        description='osu!mania 4K .osu 파일을 .sheet 파일로 변환',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
사용 예시:
  %(prog)s beatmap.osu                    # beatmap.sheet로 저장
  %(prog)s beatmap.osu -o output.sheet    # 출력 파일 지정
  %(prog)s *.osu                          # 여러 파일 일괄 변환
        """
    )

    parser.add_argument('input', nargs='+', help='변환할 .osu 파일 (여러 개 가능)')
    parser.add_argument('-o', '--output', help='출력 .sheet 파일 경로 (단일 파일 변환 시)')

    args = parser.parse_args()

    # 여러 파일 처리
    if len(args.input) > 1 and args.output:
        print("❌ 오류: 여러 파일을 변환할 때는 -o 옵션을 사용할 수 없습니다.")
        sys.exit(1)

    converter = OsuToSheetConverter()

    for i, input_file in enumerate(args.input):
        if not os.path.exists(input_file):
            print(f"❌ 파일을 찾을 수 없습니다: {input_file}")
            continue

        if len(args.input) > 1:
            print(f"\n{'='*60}")
            print(f"[{i+1}/{len(args.input)}] 변환 중...")
            print(f"{'='*60}\n")

        # 출력 파일 결정
        if args.output:
            output_file = args.output
        else:
            output_file = str(Path(input_file).with_suffix('.sheet'))

        try:
            converter.convert(input_file, output_file)
        except Exception as e:
            print(f"❌ 오류 발생: {e}")
            import traceback
            traceback.print_exc()
            continue


if __name__ == '__main__':
    main()
