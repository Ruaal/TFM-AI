import os
import json
from state.state import State
from utils.npc_loader import load_npc_config


def load_state(npc_id) -> State:
    path = os.path.join("state", f"{npc_id}_state.json")
    if not os.path.exists(path):
        state = State()
        state.npc_config = load_npc_config(npc_id)
        return state
    with open(path, "r", encoding="utf-8") as f:
        data = json.load(f)
        state = State(**data)
        return state


def save_state(npc_id, state: State):
    try:
        path = os.path.join("state", f"{npc_id}_state.json")
        os.makedirs("state", exist_ok=True)
        with open(path, "w", encoding="utf-8") as f:
            json.dump(state.__dict__, f, ensure_ascii=False, indent=2)
    except Exception as e:
        print(f"Error al guardar el estado del NPC {npc_id}: {e}")
        raise e
