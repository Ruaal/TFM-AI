from state.state import State
from utils.llm import generate_response


def mission_complete(state: State):
    config = state.npc_config
    tone = config.get("tone", "neutral")
    courtesy = config.get("courtesy", "medium")
    name = config.get("name", "NPC")
    background = config.get("history", "")
    mission = state.active_mission
    if not mission:
        state.add_message(
            role="npc", content="There is no mission to complete right now."
        )
        return state

    objectives_text = ", ".join(
        f"{o['quantity']} × {o['target']}" for o in mission.get("objectives", [])
    )
    reward_text = ", ".join(
        f"{o['quantity_reward']} × {o['reward']}" for o in mission.get("objectives", [])
    )

    system_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {state.mood}.
        Background: {background}

        The player just completed the following mission:
        - Objectives completed: {objectives_text}
        - Reward given: {reward_text}

        Respond with a short message that reflects your personality, acknowledging the player's effort and giving them the reward.
    """

    messages = [
        {"role": "system", "content": system_prompt},
        {"role": "user", "content": state.user_message},
    ]

    response = generate_response(messages)
    state.active_mission = None
    state.add_message(role="npc", content=response.content.strip())
    return state
