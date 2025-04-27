import os
import json

def load_npc_config(npc_id):
    path = os.path.join('config', f'{npc_id}.json')
    if not os.path.exists(path):
        raise Exception(f"NPC {npc_id} no encontrado")
    with open(path, 'r', encoding='utf-8') as f:
        return json.load(f)
