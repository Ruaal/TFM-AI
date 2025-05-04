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


def mission_assign(state):
    config = state["npc_config"]
    mood = state["mood"]
    tone = config.get("tone", "neutral")
    courtesy = config.get("courtesy", "medium")
    name = config.get("name", "NPC")
    background = config.get("history", "")
    user_message = state["user_message"]
    allowed_items = ", ".join(config.get("allowed_items", [])) or None
    allowed_enemies = ", ".join(config.get("allowed_enemies", [])) or None

    def fallback_response():
        fallback_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {mood}.
        Background: {background}
        The player asked for a mission, but you are unable to provide one at the moment.
        Write a polite and in-character response explaining that you cannot assign a mission right now.
        """
        fallback_messages = [
            {"role": "system", "content": fallback_prompt},
            {"role": "user", "content": user_message},
        ]
        return generate_response(fallback_messages)

    if state.get("active_mission"):
        return {
            "history": state["history"]
            + [
                {
                    "role": "npc",
                    "message": "You already have an active mission. Please complete it before asking for another task.",
                }
            ],
            "mood": state["mood"],
            "mission": state["active_mission"],
        }

    if not allowed_items and not allowed_enemies:
        response = fallback_response()
        return {
            "history": state["history"]
            + [{"role": "npc", "message": response.content.strip()}],
            "mood": mood,
            "active_mission": None,
        }

    system_prompt = f"""
        You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level.
        Your current mood is {mood}.
        Background: {background}
        The player asked for a mission. Briefly describe a task or mission you can offer, in a way that fits your personality and current mood.
        You can only assign objectives from this list:
        Items: {allowed_items}
        Enemies: {allowed_enemies}

        Respond with a short, natural-sounding message to the player, in character, as if you were speaking to them.

        Do not mention any JSON or that you will provide details.
        Do not say things like "Here are the details of your mission".
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
                        "reward": <item>,
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
        {"role": "user", "content": user_message},
    ]

    response = generate_response(messages)
    message, mission = parse_response_and_mission(response.content)
    if not mission:
        response = fallback_response()
        message = response.content.strip()

    return {
        "history": state["history"] + [{"role": "npc", "message": message}],
        "mood": mood,
        "active_mission": mission,
    }
