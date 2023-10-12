if !def(INCLUDE_7EB66C231C064695B93939D67FD6C893)

include "std.inc"

rsreset
    def InputEventType_keyDown rb 1
    def InputEventType_keyUp rb 1
    def InputEventType_mouseMove rb 1
    def InputEventType_mouseDown rb 1
    def InputEventType_mouseUp rb 1

def KeyCode_mouseLeft equ $01
def KeyCode_mouseRight equ $02
def KeyCode_mouseMiddle equ $03
def KeyCode_mouseXButton1 equ $04
def KeyCode_mouseXButton2 equ $05
def KeyCode_enter equ $0D
def KeyCode_escape equ $1B

def input_spriteCount equ 4

rsreset
    def CursorSpriteData_y rb sizeof_UInt8
    def CursorSpriteData_x rb sizeof_UInt8
    def sizeof_CursorSpriteData equ _RS

rsreset
    def SystemCursor_none rb 1
    def SystemCursor_normal rb 1
    def SystemCursor_sizeWE rb 1
    def SystemCursor_sizeNS rb 1
    def SystemCursor_sizeNESW rb 1
    def SystemCursor_sizeNWSE rb 1
    def SystemCursor_sizeAll rb 1
    def SystemCursor_iBeam rb 1
    def SystemCursor_hand rb 1
    def SystemCursor_cross rb 1
    def SystemCursor_no rb 1
    def SystemCursor_wait rb 1
    def endof_SystemCursor equ _RS

rsreset
    def MouseButtonStatesBit_left rb 1
    def MouseButtonStatesBit_right rb 1
    def MouseButtonStatesBit_middle rb 1
    assert _RS <= bitsInByte

def MouseButtonStates_none equ 0
def MouseButtonStates_left equ 1 << MouseButtonStatesBit_left
def MouseButtonStates_right equ 1 << MouseButtonStatesBit_right
def MouseButtonStates_middle equ 1 << MouseButtonStatesBit_middle

endc
