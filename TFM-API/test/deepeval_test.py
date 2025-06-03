import os
from deepeval.test_case import LLMTestCase
from deepeval.metrics import (
    AnswerRelevancyMetric,
    FaithfulnessMetric,
    ContextualPrecisionMetric,
    ContextualRecallMetric,
)
from deepeval.dataset import EvaluationDataset
from deepeval import evaluate
from dotenv import load_dotenv
import requests
import json

load_dotenv()
API_URL = "http://localhost:5000/npc/Aria/message"


def load_npc_config(npc_id):
    path = os.path.join("..", "config", f"{npc_id}.json")
    if not os.path.exists(path):
        raise Exception(f"NPC {npc_id} no encontrado")
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)


test_cases = [
    {
        "prompt": "Hello, who are you?",
        "expected_context": "The NPC should introduce themselves and possibly welcome the player.",
    },
    {
        "prompt": "Can you give me a mission?",
        "expected_context": "The NPC should offer a task or quest and the reward of it.",
    },
    {
        "prompt": "What do you know about the anomaly on this island?",
        "expected_context": "The NPC should reference the anomaly and its effect on Arkena.",
    },
    {
        "prompt": "Can you tell me a legend from Arkena?",
        "expected_context": "The NPC should share a story or legend specific to Arkena.",
    },
    {
        "prompt": "Goodbye.",
        "expected_context": "The NPC should respond with a farewell or polite closing.",
    },
]

requests.delete("http://127.0.0.1:5000/npc/Aria/state", timeout=10)
config = load_npc_config("Aria")
relevancy_metric = AnswerRelevancyMetric(threshold=0.8)
faithfulness_metric = FaithfulnessMetric(threshold=0.8)
evaluation_cases = []
for case in test_cases:
    data = {"message": case["prompt"]}
    response = requests.post(API_URL, json=data, timeout=30)
    npc_reply = response.json().get("response", "")

    evaluation_cases.append(
        LLMTestCase(
            input=case["prompt"],
            actual_output=npc_reply,
            expected_output=case["expected_context"],
            context=[
                config.get("history", ""),
                config.get("general_knowledge", ""),
            ],
            retrieval_context=[
                config.get("history", ""),
                config.get("general_knowledge", ""),
            ],
        )
    )
dataset = EvaluationDataset(test_cases=evaluation_cases)
evaluate(
    dataset,
    metrics=[relevancy_metric, faithfulness_metric],
)
