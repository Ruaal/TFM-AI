from state.state import State
from utils.llm import generate_response
import re
import json


def parse_response_and_mission(text):
    regex = r"```[\s\n]*<!-- mission\s*{.*}[\s\n]*-->[\s\n]*```"
    match = re.search(regex, text, re.DOTALL)
    if match:
        try:
            inner = re.search(r"<!-- mission\s*({.*})\s*-->", match.group(0), re.DOTALL)
            if not inner:
                return None, None

            mission_data = json.loads(inner.group(1)).get("mission", {})
            text = text.replace(match.group(0), "").strip()
            return text, mission_data

        except json.JSONDecodeError as e:
            print(f"JSON decoding error: {e}")
            return None, None

    return None, None


def mission_assign(state: State):
    config = state.npc_config
    tone = config.get("tone", "neutral")
    courtesy = config.get("courtesy", "medium")
    name = config.get("name", "NPC")
    background = config.get("history", "")
    general_knowledge = config.get("general_knowledge", "")
    allowed_items = ", ".join(config.get("allowed_items", [])) or None
    allowed_enemies = ", ".join(config.get("allowed_enemies", [])) or None
    allowed_rewards = ", ".join(config.get("allowed_rewards", [])) or None

    def fallback_response():
        fallback_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {state.mood}.
        Background: {background}
        General Knowledge of de NPC: {general_knowledge}
        The player asked for a mission, but you are unable to provide one at the moment.
        Write a polite and in-character response explaining that you cannot assign a mission right now.
        """
        fallback_messages = [
            {"role": "system", "content": fallback_prompt},
            {"role": "user", "content": state.user_message},
        ]
        return generate_response(fallback_messages)

    if state.active_mission:
        state.add_message(
            role="npc",
            content="You already have an active mission. Please complete it before asking for another task.",
        )
        return state

    if not allowed_items and not allowed_enemies and not allowed_rewards:
        response = fallback_response()
        state.add_message(
            role="npc",
            content=response.content.strip(),
        )
        return state

    system_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {state.mood}.
        Background: {background}

        This is the history of the conversation: {state.history}\n"
        The player asked for a mission. Briefly describe a task or mission you can offer, in a way that fits your personality and current mood.
        You can only assign objectives and rewards from this list:
        Allowed Items: {allowed_items}
        Allowed Enemies: {allowed_enemies}
        Allowed rewards: {allowed_rewards}

        Respond ONLY with a concise, in-character mission statement that directly describes the task and its reward, without any greetings, wishes, explanations, or extra commentary.
        Do not include any statements about earning or describing the reward, or any wishes for the player.
        Only include one kind of objective and one kind of reward in your response.

        Do not mention any JSON or that you will provide details.
        Do not say things like "Here are the details of your mission".
        Do not mention any location or place where the mission takes place.
        After your natural message, **ENSURE** to include a comment block like this **without introducing it** with the following format:
        ```
        <!-- mission
        {{
            "mission": {{
                "description": "<description>",
                "objectives": [
                    {{
                        "type": "<collect/defeat>",
                        "target": "<item/enemy>",
                        "quantity": <number>,
                        "reward": "<allowed reward>",
                        "quantity_reward": <number>
                    }}
                ]
            }}
        }}
        -->
        ```
        """

    messages = [
        {"role": "system", "content": system_prompt},
    ]

    response = generate_response(messages)
    message, mission = parse_response_and_mission(response.content)
    if not mission:
        return fallback_response()
    state.add_message(role="npc", content=message)
    state.active_mission = mission
    return state
