from flask import Flask, request, jsonify
from dotenv import load_dotenv

load_dotenv()

from flow.ai_graph import npc_graph
from state.state import State
from utils.llm import generate_transcription
from utils.state_manager import load_state, save_state
import os
import time

OPEN_API_KEY = os.getenv("OPENAI_API_KEY")
app = Flask(__name__)


@app.route("/npc/<npc_id>/message", methods=["POST"])
def interact_npc(npc_id):
    start_time = time.time()
    data = request.get_json()
    user_message = data.get("message", "")

    state = load_state(npc_id)
    state.user_message = user_message
    new_state: State = npc_graph.invoke(state)

    save_state(npc_id, new_state)
    elapsed_time = time.time() - start_time
    print(f"Processing time for NPC {npc_id}: {elapsed_time:.2f} seconds")
    return jsonify(
        {
            "user_message": user_message,
            "response": new_state.get_last_message()["content"],
            "mood": new_state.mood,
        }
    )


@app.route("/npc/<npc_id>/state", methods=["DELETE"])
def reset_npc_state(npc_id):
    state = load_state(npc_id)
    state.reset()
    save_state(npc_id, state)
    return jsonify({"message": "State reset successfully."})


@app.route("/npc/<npc_id>/mission", methods=["GET"])
def get_npc_mission(npc_id):
    state = load_state(npc_id)
    return jsonify({"mission": state.active_mission})


@app.route("/npc/<npc_id>/complete_mission", methods=["GET"])
def complete_mission(npc_id):
    state = load_state(npc_id)
    state.user_message = "I came to complete the mission."
    new_state: State = npc_graph.invoke(state)
    save_state(npc_id, new_state)

    return jsonify(
        {
            "user_message": state.user_message,
            "response": new_state.get_last_message()["content"],
            "mood": new_state.mood,
        }
    )


@app.route("/npc/<npc_id>/audio_message", methods=["POST"])
def transcribe_audio(npc_id):
    start_time = time.time()
    if "audioClip" not in request.files:
        return jsonify({"error": "No audio file provided."}), 400

    audio_file = request.files["audioClip"]
    audio_file.seek(0, os.SEEK_END)
    file_length = audio_file.tell()
    audio_file.seek(0)
    max_size = 4 * 1024 * 1024  # 4MB

    if file_length > max_size:
        return jsonify({"error": "Audio file is too large. Maximum size is 4MB."}), 400

    user_message = generate_transcription(audio_file)
    state = load_state(npc_id)
    state.user_message = user_message
    new_state: State = npc_graph.invoke(state)

    save_state(npc_id, new_state)
    elapsed_time = time.time() - start_time
    print(f"Processing time for NPC {npc_id}: {elapsed_time:.2f} seconds")
    return jsonify(
        {
            "user_message": user_message,
            "response": new_state.get_last_message()["content"],
            "mood": new_state.mood,
        }
    )


if __name__ == "__main__":
    app.run()
