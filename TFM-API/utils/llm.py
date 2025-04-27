from langchain.chat_models import init_chat_model

model = init_chat_model("gpt-4o-mini-2024-07-18", model_provider="openai")

def generate_response(messages):
    return model.invoke(messages)
