using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Int, Float, String,
    Read, Write, Repeat, Until,
    If, Elseif, Else, Then, Return, Endl, Comment,
    Semicolon, Comma,
    L_Bracket, R_Bracket, L_Curly_Bracket, R_Curly_Bracket,
    Equal, LessThan, GreaterThan, NotEqual, Assignment,
    AND, OR,
    Plus, Minus, Multiply, Divide,
    Idenifier, Constant,
    Main, End, Number
}

namespace JASON_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        //Dictionary<string, Token_Class> SpicialCharacters = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("Number", Token_Class.Number);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);

            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);

            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);

            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);

            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("end", Token_Class.End);
            ///////////////////////////////////////////////////
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);

            Operators.Add("(", Token_Class.L_Bracket);
            Operators.Add(")", Token_Class.R_Bracket);
            Operators.Add("{", Token_Class.L_Curly_Bracket);
            Operators.Add("}", Token_Class.R_Curly_Bracket);

            Operators.Add("=", Token_Class.Equal);
            Operators.Add("<", Token_Class.LessThan);
            Operators.Add(">", Token_Class.GreaterThan);
            Operators.Add("<>", Token_Class.NotEqual);
            Operators.Add(":=", Token_Class.Assignment);

            Operators.Add("+", Token_Class.Plus);
            Operators.Add("-", Token_Class.Minus);
            Operators.Add("/", Token_Class.Divide);
            Operators.Add("*", Token_Class.Multiply);

            Operators.Add("&&", Token_Class.AND);
            Operators.Add("||", Token_Class.OR);
        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                {
                    continue;
                }

                //if you read a character read until there's space or unrecognised character
                if (char.IsLetter(CurrentChar))
                {
                    // CurrentChar = SourceCode[++j];
                    while (SourceCode.Length > j && char.IsLetterOrDigit(SourceCode[j]))
                    {
                        if (j != i)
                        {
                            CurrentLexeme += SourceCode[j];
                        }
                        j++;
                        //CurrentChar = SourceCode[++j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                // number
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    //CurrentChar = SourceCode[++j];
                    while (j < SourceCode.Length && (char.IsLetterOrDigit(SourceCode[j]) || SourceCode[j] == '.'))
                    {
                        if (j != i)
                        {
                            CurrentLexeme += SourceCode[j];
                        }
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                ////////////////////////////
                //comment
                else if (CurrentChar == '/' && SourceCode[i + 1] == '*')
                {
                    while (true)
                    {
                        i++;
                        if (i >= SourceCode.Length)
                        {
                            break;
                        }
                        CurrentChar = SourceCode[i];
                        if (CurrentChar == '*' && SourceCode[i + 1] == '/')
                        {
                            CurrentLexeme += CurrentChar;
                            CurrentLexeme += SourceCode[i + 1];
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                    }
                    i++;
                    FindTokenClass(CurrentLexeme);
                }
                ////////////////////////////
                else if (CurrentChar == '\"')
                {
                    j++;
                    while (j < SourceCode.Length && SourceCode[j] != '\"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (CurrentChar == '\"')
                        CurrentLexeme += '\"';
                    FindTokenClass(CurrentLexeme);
                    i = ++j;
                }
                ///////////////////////////
                else if (CurrentChar == '{' || CurrentChar == '}')
                {
                    FindTokenClass(CurrentLexeme);
                }
                ////////////////////////////
                else if (CurrentChar == '(' || CurrentChar == ')')
                {
                    FindTokenClass(CurrentLexeme);
                }
                /////////////////////////
                else if (!char.IsLetterOrDigit(CurrentChar)) //operator
                {
                    CurrentChar = SourceCode[j];

                    while (j < SourceCode.Length - 1 && !char.IsLetterOrDigit(CurrentChar))
                    {
                        char k = SourceCode[j + 1];
                        if (CurrentChar == '<' && k == '>')
                        {
                            CurrentLexeme += k;
                            j += 2;
                            break;
                        }
                        else if (CurrentChar == ':' && k == '=')
                        {
                            CurrentLexeme += k;
                            j += 2;
                            break;
                        }
                        else if ((CurrentChar == '>' || CurrentChar == '<') && k == '=')
                        {
                            CurrentLexeme += k;
                            j += 2;
                            break;
                        }
                        #region
                        //else if (CurrentChar == '*')
                        //{
                        //    for (int l = j; l < SourceCode.Length; l++)
                        //    {
                        //        CurrentChar = SourceCode[l];
                        //        if (CurrentChar != '*')
                        //        {
                        //            CurrentLexeme += CurrentChar;
                        //        }
                        //        else if (CurrentChar == '*')
                        //        {
                        //            CurrentLexeme += CurrentChar;
                        //            j = l + 1;
                        //            break;
                        //        }
                        //        else if (l == SourceCode.Length - 1)
                        //        {
                        //            CurrentLexeme = "*";
                        //            break;
                        //        }
                        //    }
                        //}
                        #endregion
                        else if (Operators.ContainsKey(CurrentLexeme))
                        {
                            j++;
                            break;
                        }
                        else if (j != i)
                        {
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar;
                        }
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }

                // FindTokenClass(CurrentLexeme.Trim());
            }
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?

            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];

                Tokens.Add(Tok);

            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);

            }
            //Is it a Number?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a String?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            // is it comment?
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            #region
            //Is it an operator?
            //else if (SpicialCharacters.ContainsKey(Lex))
            //{
            //    Tok.token_type = SpicialCharacters[Lex];
            //    Tokens.Add(Tok);
            //}
            //Is it an undefined?
            //Is it an undefined?
            #endregion
            else
            {
                Errors.Error_List.Add("Undefined Token " + Lex);
            }
        }
        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.
            //updated.................var rx = new Regex(@"^[a-zA-z_][a-zA-z0-9_]*$", RegexOptions.Compiled);
            //starting with char or underscore ..must end with char or underscore
            var rx = new Regex(@"^[A-Za-z ][-a-zA-Z0-9]*$", RegexOptions.Compiled);
            //if (rx.IsMatch(lex))
            //{
            //    isValid = true;
            //}
            isValid = rx.IsMatch(lex);
            return isValid;
        }

        bool isNumber(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.
            //starting with a number aat least one ..must end with number
            //updated .............. var rx = new Regex(@"^[0-9]+$", RegexOptions.Compiled);
            //var rx = new Regex(@"[+| -] ? [0-9]+ [\.  [0-9]+]?", RegexOptions.Compiled);
            var rx = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            //if (rx.IsMatch(lex))
            //{
            //    isValid = true;
            //}
            isValid = rx.IsMatch(lex);
            return isValid;
        }

        bool isComment(string lex)
        {
            bool isValid = true;
            // Check if the lex is a comment or not.
            //updated................. var rx = new Regex(@"^/\[\s|\S]?\*/", RegexOptions.Compiled);
            var rx = new Regex("^(\\/*).*(\\*/)$", RegexOptions.Compiled);
            //if (rx.IsMatch(lex))
            //{
            //    isValid = true;
            //}
            isValid = rx.IsMatch(lex);
            return isValid;
        }

        bool isString(string lex)
        {
            bool isValid = true;
            //starting with double quotes ..accept any sequence of chars..must end with double quotes.
            var rx = new Regex("^\".*\"$", RegexOptions.Compiled);

            //if (rx.IsMatch(lex))
            //{
            //    isValid = true;
            //}
            isValid = rx.IsMatch(lex);
            return isValid;
        }
    }
}
