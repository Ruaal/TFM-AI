from state.state import State
from utils.llm import generate_response


def bad_intent_response(state: State):
    config = state.npc_config
    tone = config.get("tone", "neutral")
    courtesy = config.get("courtesy", "medium")
    name = config.get("name", "NPC")
    background = config.get("history", "")
    general_knowledge = config.get("general_knowledge", "")

    system_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {state.mood}.
        Background: {background}.
        This is the history of the conversation: {state.history}
        The player insulted you or was disrespectful. Generate an appropriate short response based on your personality and mood.
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
