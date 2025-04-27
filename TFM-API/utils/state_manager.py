import os
import json

def load_state(npc_id):
    path = os.path.join('state', f'{npc_id}_state.json')
    if not os.path.exists(path):
        return {"history": [], "mood": "neutral"}
    with open(path, 'r', encoding='utf-8') as f:
        return json.load(f)

def save_state(npc_id, state):
    try:
        path = os.path.join('state', f'{npc_id}_state.json')
        os.makedirs('state', exist_ok=True)
        with open(path, 'w', encoding='utf-8') as f:
            json.dump(state, f, ensure_ascii=False, indent=2)
    except Exception as e:
        print(f"Error al guardar el estado del NPC {npc_id}: {e}")
        raise e
