import keyboard
import struct

f = open(r'\\.\pipe\InputHandlerSnake', 'r+b', 0)

# ask for input and put it in correct format 1up,2right,3down,4left
def handleUpKey(e):
    f.write(bytes("1", 'utf-8'))
    print("1")
    f.seek(0)
def handleRightKey(e):
    f.write(bytes("2", 'utf-8'))
    print("2")
    f.seek(0)
def handleDownKey(e):
    f.write(bytes("3", 'utf-8'))
    print("3")
    f.seek(0)
def handleLeftKey(e):
    f.write(bytes("4", 'utf-8'))
    print("4")
    f.seek(0)

keyboard.on_press_key("w", handleUpKey)
keyboard.on_press_key("d", handleRightKey)
keyboard.on_press_key("s", handleDownKey)
keyboard.on_press_key("a", handleLeftKey)

while True:
    pass