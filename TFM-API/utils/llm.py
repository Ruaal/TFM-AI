import io
from langchain.chat_models import init_chat_model
from openai import OpenAI

model = init_chat_model("gpt-4o-mini-2024-07-18", model_provider="openai")
client = OpenAI()

def generate_response(messages):
    return model.invoke(messages)

def generate_transcription(audio_file):
    audio_bytes = audio_file.read()
    # Wrap in BytesIO and set .name to help OpenAI detect the format
    audio_stream = io.BytesIO(audio_bytes)
    audio_stream.name = getattr(audio_file, 'filename', 'audio.mp3')  # fallback to 'audio.mp3'
    transcription = client.audio.transcriptions.create(
        file=audio_stream,
        model="whisper-1",
        response_format="text"
    )
    return transcription