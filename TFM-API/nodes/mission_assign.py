from utils.llm import generate_response

def mission_assign(state):
    config = state['npc_config']
    mood = state['mood']
    tone = config.get('tone', 'neutral')
    courtesy = config.get('courtesy', 'medium')
    name = config.get('name', 'NPC')
    background = config.get('history', '')
    user_message = state['user_message']

    system_prompt = (
        f"You are {name}, an NPC with a {tone} tone and {courtesy} courtesy level. "
        f"Your current mood is {mood}. "
        f"Background: {background}\n"
        "The player asked for a mission. Briefly describe a task or mission you can offer, in a way that fits your personality and current mood."
    )

    messages = [{"role": "system", "content": system_prompt},
            {"role": "user", "content": user_message}]

    response = generate_response(messages)

    return {"history": state['history'] + [{"role": "npc", "message": response.content.strip()}], "mood": mood}
