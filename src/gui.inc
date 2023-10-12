if !def(INCLUDE_6F074EBC96D1462B9CB979857D6E5F65)
def INCLUDE_6F074EBC96D1462B9CB979857D6E5F65 equ 1

rsreset
    def ControlHandle_null rb 1
    def ControlHandle_first rb 1

rsreset
    def ControlType_null rb 1
    def ControlType_window rb 1

rsreset
    def EventType_null rb 1
    def EventType_paint rb 1

rsreset
    def sizeof_ControlFlagBit_type equ 4 ;* @type ControlType
    def ControlFlagBit_type rb sizeof_ControlFlagBit_type
    def sizeof_ControlFlagBit_unused equ 3
    def ControlFlagBit_unused rb sizeof_ControlFlagBit_unused
    def ControlFlagBit_exists rb 1
    assert _RS <= bitsInByte
def ControlFlag_type equ ((1 << sizeof_ControlFlagBit_type) - 1) << ControlFlagBit_type
def ControlFlag_unused equ ((1 << sizeof_ControlFlagBit_unused) - 1) << ControlFlagBit_unused
def ControlFlag_exists equ 1 << ControlFlagBit_exists

rsreset
    def Control_state rb sizeof_UInt8 ;* @type ControlFlag
    def Control_x rb sizeof_UInt8
    def Control_y rb sizeof_UInt8
    def Control_width rb sizeof_UInt8
    def Control_height rb sizeof_UInt8
    def Control_eventHandler rb sizeof_Ptr16 ;* @type void(*)(EventType eventType<a>)
    def actualsizeof_Control equ _RS
    def sizeof_Control equ 16
    assert actualsizeof_Control <= sizeof_Control
    def alignof_Control equ 4

rsreset
    def WindowStyle_border rb 1
    def WindowStyle_borderless rb 1
    def endof_WindowStyle equ _RS

rsreset
    def sizeof_WindowFlagBit_style equ 1
    assert endof_WindowStyle <= 1 << sizeof_WindowFlagBit_style
    def WindowFlagBit_style rb sizeof_WindowFlagBit_style ;* @type WindowStyle
    def WindowFlagBit_backgroundEnabled rb 1
    ;* Whether the window stays in place, even when tring to bring it to the front.
    def WindowFlagBit_staysInBack rb 1
    def WindowFlagBit_resizable rb 1
    assert _RS <= bitsInByte
def WindowFlag_style equ ((1 << sizeof_WindowFlagBit_style) - 1) << WindowFlagBit_style
def WindowFlag_backgroundEnabled equ 1 << WindowFlagBit_backgroundEnabled
def WindowFlag_staysInBack equ 1 << WindowFlagBit_staysInBack
def WindowFlag_resizable equ 1 << WindowFlagBit_resizable

rsset actualsizeof_Control
    def Window_flags rb sizeof_UInt8 ;* @type WindowFlag
    def Window_title rb sizeof_Ptr16 ;* @type string*
    def Window_iconBank rb sizeof_UInt8
    def Window_icon rb sizeof_Ptr16 ;* @type Image*
    def sizeof_Window equ sizeof_Control
    def alignof_Window equ alignof_Control
    assert sizeof_Window <= sizeof_Control

def gui_titlebarButtonCount equ 3
def gui_taskbarHeight equ 9

endc
