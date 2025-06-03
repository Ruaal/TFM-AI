from state.state import State


def fallback_response(state: State):
    state.add_message(
        role="npc",
        content="I didn't understand what you said.",
    )
    return state
