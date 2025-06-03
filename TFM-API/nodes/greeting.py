from state.state import State
from utils.llm import generate_response


def greeting_response(state: State):
    config = state.npc_config
    tone = config.get("tone", "neutral")
    courtesy = config.get("courtesy", "medium")
    name = config.get("name", "NPC")
    background = config.get("history", "")

    system_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {state.mood}.
        Background: {background}
        This is the history of the conversation: {state.history}
        The player greeted you. Generate a short greeting response that directly introduces your identity (name and role) based on your personality and mood. Avoid adding irrelevant background or setting details; keep the response concise and focused on greeting and identity.
    """

    messages = [
        {"role": "system", "content": system_prompt},
    ]

    response = generate_response(messages)
    state.add_message(
        role="npc",
        content=response.content.strip(),
    )

    return state
