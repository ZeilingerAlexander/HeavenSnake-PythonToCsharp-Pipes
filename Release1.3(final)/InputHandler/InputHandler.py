import keyboard
import time

f = open(r'\\.\pipe\InputHandlerSnakes', 'r+b', 0)

# ask for input and put it in correct format 1up,2right,3down,4left
def handleUpKey(e):
    f.write(bytes("1", 'utf-8'))
    f.seek(0)
def handleRightKey(e):
    f.write(bytes("2", 'utf-8'))
    f.seek(0)
def handleDownKey(e):
    f.write(bytes("3", 'utf-8'))
    f.seek(0)
def handleLeftKey(e):
    f.write(bytes("4", 'utf-8'))
    f.seek(0)
def handleSpaceKey(e):
    f.write(bytes("5", "utf-8"))
    f.seek(0)

keyboard.on_press_key("w", handleUpKey)
keyboard.on_press_key("d", handleRightKey)
keyboard.on_press_key("s", handleDownKey)
keyboard.on_press_key("a", handleLeftKey)
keyboard.on_press_key("space", handleSpaceKey)

while True:
    time.sleep(1000000)
    pass