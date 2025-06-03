class State:
    def __init__(self, history=[], mood="neutral", active_mission=None, npc_config=None, user_message="", intent="OTHER"):
        self.history = history
        self.mood = mood
        self.active_mission = active_mission
        self.npc_config = npc_config
        self.user_message = user_message
        self.intent = intent

    def add_message(self, role, content):
        self.history.append({"role": role, "content": content})

    def get_last_message(self):
        if self.history:
            return self.history[-1]
        return None

    def reset(self):
        self.history = []
        self.mood = "neutral"
        self.active_mission = None
        self.user_message = ""
