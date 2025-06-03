from state.state import State


def update_mood(state: State):
    current_mood = state.mood
    intent = state.intent
    mood_transitions = {
        "content": "neutral",
        "neutral": "irritated",
        "irritated": "furious",
    }

    inverse_transitions = {
        "furious": "irritated",
        "irritated": "neutral",
        "neutral": "content",
    }

    if intent == "BAD_INTENT":
        state.mood = mood_transitions.get(current_mood, current_mood)
    elif intent == "COMPLETE_MISSION":
        state.mood = inverse_transitions.get(current_mood, current_mood)

    return state
