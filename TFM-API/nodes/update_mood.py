def update_mood(state):
    current_mood = state.get('mood', 'neutral')
    intent = state.get('intent', 'OTHER')
    mood_transitions = {
        'content': 'neutral',
        'neutral': 'irritated',
        'irritated': 'furious'
    }

    if intent == 'BAD_INTENT':
        state['mood'] = mood_transitions.get(current_mood, current_mood)
        
    return state
