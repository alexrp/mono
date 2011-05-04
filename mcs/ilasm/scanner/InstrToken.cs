// InstrToken.cs
// Author: Sergey Chaban (serge@wildwestsoftware.com)
using System;
using System.Reflection.Emit;

namespace Mono.ILAsm {
	public class InstrToken : ILToken {
		public InstrToken (OpCode opcode)
		{
			this.val = opcode;
			this.token = GetInstrType (opcode);
		}

		public static int GetInstrType (OpCode opCode)
		{
			switch (opCode.OperandType) {
			case OperandType.InlineBrTarget:
			case OperandType.ShortInlineBrTarget:
				return Token.INSTR_BRTARGET;
			case OperandType.InlineField:
				return Token.INSTR_FIELD;
			case OperandType.InlineI:
			case OperandType.ShortInlineI:
				return Token.INSTR_I;
			case OperandType.InlineI8:
				return Token.INSTR_I8;
			case OperandType.InlineMethod:
				return Token.INSTR_METHOD;
			case OperandType.InlineNone:
				return Token.INSTR_NONE;
#pragma warning disable 0618
			case OperandType.InlinePhi:
				return Token.INSTR_PHI;
#pragma warning restore 0618
			case OperandType.InlineR:
			case OperandType.ShortInlineR:
				return Token.INSTR_R;
			case OperandType.InlineSig:
				return Token.INSTR_SIG;
			case OperandType.InlineString:
				return Token.INSTR_STRING;
			case OperandType.InlineSwitch:
				return Token.INSTR_SWITCH;
			case OperandType.InlineTok:
				return Token.INSTR_TOK;
			case OperandType.InlineType:
				return Token.INSTR_TYPE;
			case OperandType.InlineVar:
			case OperandType.ShortInlineVar:
				return Token.INSTR_VAR;
			}
			
			return Token.UNKNOWN;
		}
	}
}
