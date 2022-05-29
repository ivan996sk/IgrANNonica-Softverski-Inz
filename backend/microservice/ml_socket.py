import asyncio
import websockets
import json

def get_or_create_eventloop():
    try:
        return asyncio.get_event_loop()
    except RuntimeError as ex:
        if "There is no current event loop in thread" in str(ex):
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            return asyncio.get_event_loop()

# create handler for each connection
async def handler(websocket, path):
    #data = json.loads(await websocket.recv())
    #print(data['test'])
    msg = await websocket.recv()
    print(msg)

async def start():
    start_server = websockets.serve(handler, "localhost", 5027)
    print('Websocket starting...')
    get_or_create_eventloop().run_until_complete(start_server)
    get_or_create_eventloop().run_forever()


async def send(msg):
    print("WS sending message:")
    print(msg)
    await websocket.send(msg)
