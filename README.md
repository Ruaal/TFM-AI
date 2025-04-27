# TFM-AI

This repository contains two main parts:
- `TFM-API/`: Python Flask backend for NPC dynamic conversations and missions.
- `TFM-Game/`: Unity game where players interact with the NPCs via the API.

## Requirements

You need to have installed:
- Python 3.10+
- pip
- (Optional but recommended) virtualenv
- Unity 2022.3 LTS or higher

## Setting up the API

1. Navigate to the `TFM-API` folder:

    ```bash
    cd TFM-API
    ```

2. (Optional) Create and activate a virtual environment:

    ```bash
    python -m venv venv
    source venv/bin/activate        # Linux/Mac
    venv\Scripts\activate.bat        # Windows
    ```

3. Install the required packages:

    ```bash
    pip install -r requirements.txt
    ```

## Running the API

1. Inside the `TFM-API` folder, run:

    ```bash
    python app.py
    ```

2. The server will start, usually accessible at:

    ```
    http://localhost:5000
    ```

## Running the Game

1. Open the `TFM-Game/` folder using Unity Hub or directly from Unity (recommended version 2022.3 or higher).
2. Make sure the API server is **running** before launching the game.
3. Press **Play** in the Unity Editor to start testing.

## 📋 API Endpoints Overview

| Endpoint | Method | Description |
|:---------|:-------|:------------|
| `/npc/<npc_id>/message` | `POST` | Send a message to a specific NPC and receive a response. |
| `/npc/<npc_id>/state` | `DELETE` | Reset the NPC's state (clear history and set mood to neutral). |

### Example of sending a message

```bash
POST http://localhost:5000/npc/Aria/message
Content-Type: application/json

{
  "message": "Hello, who are you?"
}
```

### Resetting NPC state
```bash
DELETE http://localhost:5000/npc/Aria/state
```