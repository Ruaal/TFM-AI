from state.state import State
from utils.llm import generate_response


def detect_intent_with_llm(state: State):
    message = state.user_message
    system_prompt = f"""
        Classify the intention of the following player message.
        Options: GREETING, BAD_INTENT, ASK_MISSION, COMPLETE_MISSION, FAREWELL, CHITCHAT, OTHER.
        Only classify as ASK_MISSION if the user explicitly asks for help or requests a mission (e.g., "Can you help me?" or "Do you have a mission for me?"). 
        General questions or chitchat should not be classified as ASK_MISSION.
        Message: {message}
        Response with only one of the options.
    """
    messages = [
        {"role": "system", "content": "You are an intent classifier."},
        {"role": "user", "content": system_prompt},
    ]
    response = generate_response(messages)

    intent = response.content.strip().upper()
    valid = {
        "GREETING",
        "BAD_INTENT",
        "ASK_MISSION",
        "COMPLETE_MISSION",
        "FAREWELL",
        "CHITCHAT",
        "OTHER",
    }
    state.add_message("user", message)
    state.intent = intent if intent in valid else "OTHER"
    return state
