from utils.llm import generate_response

def detect_intent_with_llm(state):
    message = state['user_message']
    system_prompt = (
        "Classify the intention of the following player message.\n"
        "Options: GREETING, BAD_INTENT, ASK_MISSION, COMPLETE_MISSION, FAREWELL, CHITCHAT, OTHER.\n"
        f"Message: {message}\n"
        "Response with only one of the options."
    )
    messages=[{"role": "system", "content": "Eres un clasificador de intenciones."},
                  {"role": "user", "content": system_prompt}]
    response = generate_response(messages)


    intent = response.content.strip().upper()
    valid = {"GREETING", "BAD_INTENT", "ASK_MISSION", "COMPLETE_MISSION", "FAREWELL", "CHITCHAT", "OTHER"}

    state['intent'] = intent if intent in valid else "OTHER"
    return state
