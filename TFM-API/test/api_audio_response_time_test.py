import requests
import time
import random
import statistics
import os

API_URL = "http://localhost:5000/npc/Aria/audio_message"

audio_files = [
    "audio1.wav",
    "audio2.wav",
    "audio3.wav",
    "audio4.wav",
    "audio5.wav",
    "audio6.wav",
    "audio7.wav",
    "audio8.wav",
    "audio9.wav",
    "audio10.wav",
]

audio_files = [os.path.join("audio_files", f) for f in audio_files]

response_times = []
failures = 0

for i in range(1, 31):
    file = random.choice(audio_files)
    with open(file, "rb") as f:
        files = {"audioClip": (os.path.basename(file), f, "audio/wav")}
        start = time.perf_counter()
        try:
            response = requests.post(API_URL, files=files, timeout=60)
            elapsed = time.perf_counter() - start
            response.raise_for_status()
            print(
                f"[{i:02}] {os.path.basename(file):10} | {elapsed:.2f}s | Status: {response.status_code}"
            )
            response_times.append(elapsed)
        except Exception as e:
            print(f"[{i:02}] ERROR: {e}")
            failures += 1

if response_times:
    print("\n=== API Audio Response Time Stats ===")
    print(f"Samples: {len(response_times)} / 30")
    print(f"Min:   {min(response_times):.2f}s")
    print(f"Max:   {max(response_times):.2f}s")
    print(f"Mean:  {statistics.mean(response_times):.2f}s")
    print(
        f"Stdev: {statistics.stdev(response_times):.2f}s"
        if len(response_times) > 1
        else ""
    )
    print(f"Failures: {failures}")
else:
    print("No successful responses received.")
