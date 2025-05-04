from flask import Flask, request, jsonify
from flow.ai_graph import npc_graph
from utils.npc_loader import load_npc_config
from utils.state_manager import load_state, save_state
from dotenv import load_dotenv
import os

load_dotenv()
OPEN_API_KEY = os.getenv("OPENAI_API_KEY")
app = Flask(__name__)


@app.route("/npc/<npc_id>/message", methods=["POST"])
def interact_npc(npc_id):
    data = request.get_json()
    user_message = data.get("message", "")

    npc_config = load_npc_config(npc_id)
    state = load_state(npc_id)
    state["npc_config"] = npc_config
    state["user_message"] = user_message

    new_state = npc_graph.invoke(state)

    save_state(npc_id, new_state)

    npc_response = new_state["history"][-1]["message"]

    return jsonify({"response": npc_response, "mood": new_state.get("mood")})


@app.route("/npc/<npc_id>/state", methods=["DELETE"])
def reset_npc_state(npc_id):
    state = load_state(npc_id)
    state["history"] = []
    state["mood"] = "neutral"
    state["active_mission"] = None
    save_state(npc_id, state)
    return jsonify({"message": "State reset successfully."})


@app.route("/npc/<npc_id>/mission", methods=["GET"])
def get_npc_mission(npc_id):
    state = load_state(npc_id)
    mission = state.get("active_mission")
    return jsonify({"mission": mission})


if __name__ == "__main__":
    app.run(debug=True)
