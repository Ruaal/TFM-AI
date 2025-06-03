import requests
import time
import random
import statistics

API_URL = "http://localhost:5000/npc/Aria/message"

messages = [
    "Hi.",
    "Hello, can you help me?",
    "Tell me about this island.",
    "Do you have any missions for me?",
    "What's your name?",
    "Can you share a legend from Arkena?",
    "How many guardians live here?",
    "What dangers should I avoid in the forest?",
    "Explain the history of the magical runes in detail.",
    "Please tell me everything you know about the ancient arcane forces and the true nature of the anomaly that caused my arrival, including legends, events, and any warnings you have for travelers like me.",
]

response_times = []
failures = 0

for i in range(1, 31):
    msg = random.choice(messages)
    data = {"message": msg}
    try:
        start_time = time.time()
        response = requests.post(API_URL, json=data, timeout=60)
        elapsed = time.time() - start_time
        response.raise_for_status()
        print(
            f"[{i:02}] Length: {len(msg):4} chars | {elapsed:.2f}s | Status: {response.status_code}"
        )
        response_times.append(elapsed)
    except Exception as e:
        print(f"[{i:02}] ERROR: {e}")
        failures += 1

if response_times:
    print("\n=== API Response Time Stats ===")
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
