from utils.llm import generate_response

def bad_intent_response(state):
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
        "The player insulted you or was disrespectful. Generate an appropriate short response based on your personality and mood."
    )

    messages = [{"role": "system", "content": system_prompt},
                {"role": "user", "content": user_message}]

    response = generate_response(messages)

    new_history = state['history'] + [
        {"role": "user", "message": user_message},
        {"role": "npc", "message": response.content.strip()}
    ]

    return {"history": new_history, "mood": mood}
