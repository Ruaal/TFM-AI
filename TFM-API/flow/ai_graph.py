from langgraph.graph import StateGraph, END
from nodes.intent_detection import detect_intent_with_llm
from nodes.update_mood import update_mood
from nodes.greeting import greeting_response
from nodes.bad_intent import bad_intent_response
from nodes.mission_assign import mission_assign
from nodes.mission_complete import mission_complete
from nodes.chitchat import chitchat_response
from nodes.fallback import fallback_response

graph = StateGraph(dict)

graph.add_node("DetectIntent", detect_intent_with_llm)
graph.add_node("UpdateMood", update_mood)
graph.add_node("Greeting", greeting_response)
graph.add_node("BadIntent", bad_intent_response)
graph.add_node("MissionAssign", mission_assign)
graph.add_node("MissionComplete", mission_complete)
graph.add_node("Chitchat", chitchat_response)
graph.add_node("Fallback", fallback_response)

graph.set_entry_point("DetectIntent")
graph.add_edge("DetectIntent", "UpdateMood")

def router(state):
    intent = state['intent']
    match intent:
        case "GREETING":
            return "Greeting"
        case "BAD_INTENT":
            return "BadIntent"
        case "ASK_MISSION":
            return "MissionAssign"
        case "COMPLETE_MISSION":
            return "MissionComplete"
        case "CHITCHAT":
            return "Chitchat"
        case _:
            return "Fallback"

graph.add_conditional_edges("UpdateMood", router)
graph.add_edge("Greeting", END)
graph.add_edge("BadIntent", END)
graph.add_edge("MissionAssign", END)
graph.add_edge("MissionComplete", END)
graph.add_edge("Chitchat", END)
graph.add_edge("Fallback", END)

npc_graph = graph.compile()
