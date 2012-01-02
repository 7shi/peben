module PELib.MIPS32

open System
open System.Collections.Generic

type Reg(r) =
    member x.Value = r
    override x.GetHashCode () = r
    override x.Equals (obj:obj) =
        obj :? Reg && x.Value = (obj :?> Reg).Value

let r0 , r1 , r2 , r3  = Reg  0, Reg  1, Reg  2, Reg  3
let r4 , r5 , r6 , r7  = Reg  4, Reg  5, Reg  6, Reg  7
let r8 , r9 , r10, r11 = Reg  8, Reg  9, Reg 10, Reg 11
let r12, r13, r14, r15 = Reg 12, Reg 13, Reg 14, Reg 15
let r16, r17, r18, r19 = Reg 16, Reg 17, Reg 18, Reg 19
let r20, r21, r22, r23 = Reg 20, Reg 21, Reg 22, Reg 23
let r24, r25, r26, r27 = Reg 24, Reg 25, Reg 26, Reg 27
let r28, r29, r30, r31 = Reg 28, Reg 29, Reg 30, Reg 31

let zero, at = r0, r1
let v0, v1 = r2, r3
let a0, a1, a2, a3 = r4, r5, r6, r7
let t0, t1, t2, t3, t4, t5, t6, t7, t8, t9 =
    r8, r9, r10, r11, r12, r13, r14, r15, r24, r25
let s0, s1, s2, s3, s4, s5, s6, s7, s8 =
    r16, r17, r18, r19, r20, r21, r22, r23, r30
let k0, k1 = r26, r27
let gp, sp, fp, ra = r28, r29, r30, r31

let inline add32 (list:List<byte>) el (v:int) =
    let bytes = BitConverter.GetBytes v
    if not el then Array.Reverse bytes
    list.AddRange bytes

let inline opr (list:List<byte>) el op (s:Reg) (t:Reg) (d:Reg) sh f =
    add32 list el ((op <<< 26) |||
                   (s.Value <<< 21) |||
                   (t.Value <<< 16) |||
                   (d.Value <<< 11) |||
                   ((sh &&& 31) <<< 6) |||
                   (f &&& 63))

let inline opi (list:List<byte>) el op (s:Reg) (t:Reg) i =
    add32 list el ((op <<< 26) |||
                   (s.Value <<< 21) |||
                   (t.Value <<< 16) |||
                   (i &&& 0xffff))

let inline opj (list:List<byte>) el op ad =
    add32 list el ((op <<< 26) ||| (ad &&& 0x03ffffff))

type Assembler(list:List<byte>, el:bool) =
    member x.Noop         = list.AddRange [ 0uy; 0uy; 0uy; 0uy ]
    member x.Sll    d t h = opr list el 0 r0 t  d  h 0b000000
    member x.Srl    d t h = opr list el 0 r0 t  d  h 0b000010
    member x.Sra    d t h = opr list el 0 r0 t  d  h 0b000011
    member x.Sllv   d t s = opr list el 0 s  t  d  0 0b000100
    member x.Srlv   d t s = opr list el 0 s  t  d  0 0b000110
    member x.Jr     s     = opr list el 0 s  r0 r0 0 0b001000
    member x.Syscall      = opr list el 0 r0 r0 r0 0 0b001100
    member x.Mfhi   d     = opr list el 0 r0 r0 d  0 0b010000
    member x.Mflo   d     = opr list el 0 r0 r0 d  0 0b010010
    member x.Mult   s t   = opr list el 0 s  t  r0 0 0b011000
    member x.Multu  s t   = opr list el 0 s  t  r0 0 0b011001
    member x.Div    s t   = opr list el 0 s  t  r0 0 0b011010
    member x.Divu   s t   = opr list el 0 s  t  r0 0 0b011011
    member x.Add    d s t = opr list el 0 s  t  d  0 0b100000
    member x.Addu   d s t = opr list el 0 s  t  d  0 0b100001
    member x.Sub    d s t = opr list el 0 s  t  d  0 0b100010
    member x.Subu   d s t = opr list el 0 s  t  d  0 0b100011
    member x.And    d s t = opr list el 0 s  t  d  0 0b100100
    member x.Or     d s t = opr list el 0 s  t  d  0 0b100101
    member x.Xor    d s t = opr list el 0 s  t  d  0 0b100110
    member x.Slt    d s t = opr list el 0 s  t  d  0 0b101010
    member x.Sltu   d s t = opr list el 0 s  t  d  0 0b101011

    member x.Bltz   s   o = opi list el 0b000001 s  r0  o
    member x.Bgez   s   o = opi list el 0b000001 s  r1  o
    member x.Bltzal s   o = opi list el 0b000001 s  r16 o
    member x.Bgezal s   o = opi list el 0b000001 s  r17 o
    member x.Beq    s t o = opi list el 0b000100 s  t   o
    member x.Bne    s t o = opi list el 0b000101 s  t   o
    member x.Blez   s   o = opi list el 0b000110 s  r0  o
    member x.Bgtz   s   o = opi list el 0b000111 s  r0  o
    member x.Addi   t s i = opi list el 0b001000 s  t   i
    member x.Addiu  t s i = opi list el 0b001001 s  t   i
    member x.Slti   t s i = opi list el 0b001010 s  t   i
    member x.Sltiu  t s i = opi list el 0b001011 s  t   i
    member x.Andi   t s i = opi list el 0b001100 s  t   i
    member x.Ori    t s i = opi list el 0b001101 s  t   i
    member x.Xori   t s i = opi list el 0b001110 s  t   i
    member x.Lui    t   i = opi list el 0b001111 r0 t   i
    member x.Lb     t o s = opi list el 0b100000 s  t   o
    member x.Lw     t o s = opi list el 0b100011 s  t   o
    member x.Sb     t o s = opi list el 0b101000 s  t   o
    member x.Sw     t o s = opi list el 0b101011 s  t   o

    member x.J      ad    = opj list el 0b000010 ad
    member x.Jal    ad    = opj list el 0b000011 ad

    member x.Move   t s   = x.Addi t s 0
    member x.La     at ad = x.Lui at (ad >>> 16); x.Ori at at ad
    member x.Li     at i  = let hi = int(uint32(i) >>> 16)
                            if hi <> 0 then x.Lui at hi
                            x.Ori at at i
    member x.Beqz   s   o = x.Beq s zero o
    member x.Bnez   s   o = x.Bne s zero o
    member x.Bgt    s t o = x.Slt  at t s; x.Bne at zero o
    member x.Blt    s t o = x.Slt  at s t; x.Bne at zero o
    member x.Bge    s t o = x.Slt  at s t; x.Beq at zero o
    member x.Ble    s t o = x.Slt  at t s; x.Beq at zero o
    member x.Bgtu   s t o = x.Sltu at t s; x.Bne at zero o
    member x.Bltu   s t o = x.Sltu at s t; x.Bne at zero o
    member x.Bgeu   s t o = x.Sltu at s t; x.Beq at zero o
    member x.Bleu   s t o = x.Sltu at t s; x.Beq at zero o
