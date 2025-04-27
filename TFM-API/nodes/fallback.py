def fallback_response(state):
    return {"history": state['history'] + [{"role": "npc", "message": "No entendí muy bien lo que quieres decir."}], "mood": state['mood']}
