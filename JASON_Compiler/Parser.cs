using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }

        // Program → Function_Statement Main_Function
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Function_Statement());
            program.Children.Add(Main_Function());
            MessageBox.Show("Success");
            return program;
        }

        // Function_Statement → Function_Declaration Function_Body
        Node Function_Statement()
        {
            Node function_statement = new Node("Function_Statement");
            function_statement.Children.Add(Function_Declaration());
            //function_statement.Children.Add(Function_Body());
            return function_statement;
        }

        // Function_Declaration → Datatype Function_Name ( ------ )
        Node Function_Declaration()
        {
            Node function_declaration = new Node("Function_Declaration");
            function_declaration.Children.Add(Data_Type());
            function_declaration.Children.Add(Function_Name());
            function_declaration.Children.Add(match(Token_Class.L_Bracket));
            //function_declaration.Children.Add();
            function_declaration.Children.Add(match(Token_Class.R_Bracket));
            return function_declaration;
        }

        // Function_Name → Identifier
        Node Function_Name()
        {
            Node function_name = new Node("Function_Name");
            function_name.Children.Add(match(Token_Class.Idenifier));
            return function_name;
        }

        // Function_Body → { Statments Return_Statement }
        Node Function_Body()
        {
            Node function_body = new Node("Function_Body");
            function_body.Children.Add(match(Token_Class.L_Curly_Bracket));
            //function_body.Children.Add(Statments());
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.R_Curly_Bracket));
            return function_body;
        }

        // Main_Function → Datatype main() Function_Body
        Node Main_Function()
        {
            Node main_function = new Node("Main_Function");
            main_function.Children.Add(Data_Type());
            main_function.Children.Add(match(Token_Class.Main));
            main_function.Children.Add(match(Token_Class.L_Bracket));
            main_function.Children.Add(match(Token_Class.R_Bracket));
            main_function.Children.Add(Function_Body());
            return main_function;
        }

        // Arithmatic_Operator → + | - | * | / 
        Node Arithmatic_Operator()
        {
            Node arithmatic_Operator = new Node("Arithmatic_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.Plus && InputPointer < TokenStream.Count)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.Plus));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Minus && InputPointer < TokenStream.Count)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.Minus));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Multiply && InputPointer < TokenStream.Count)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.Multiply));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Divide && InputPointer < TokenStream.Count)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.Divide));
            }
            return arithmatic_Operator;
        }

        // Boolean_Operator → AND | OR
        Node Boolean_Operator()
        {
            Node boolean_operator = new Node("Boolean_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.AND && InputPointer < TokenStream.Count)
            {
                boolean_operator.Children.Add(match(Token_Class.AND));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.OR && InputPointer < TokenStream.Count)
            {
                boolean_operator.Children.Add(match(Token_Class.OR));
            }
            return boolean_operator;
        }

        // Condition_Operator → < | > | = | <>
        Node Condition_Operator()
        {
            Node condition_operator = new Node("Condition_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.Equal && InputPointer < TokenStream.Count)
            {
                condition_operator.Children.Add(match(Token_Class.Equal));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThan && InputPointer < TokenStream.Count)
            {
                condition_operator.Children.Add(match(Token_Class.GreaterThan));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.LessThan && InputPointer < TokenStream.Count)
            {
                condition_operator.Children.Add(match(Token_Class.LessThan));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqual && InputPointer < TokenStream.Count)
            {
                condition_operator.Children.Add(match(Token_Class.NotEqual));
            }
            return condition_operator;
        }


        // Datatype → int | float | string 
        Node Data_Type()
        {
            Node data_type =  new Node("Data_Type");
            if (TokenStream[InputPointer].token_type == Token_Class.Int && InputPointer < TokenStream.Count)
            {
                data_type.Children.Add(match(Token_Class.Int));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Float && InputPointer < TokenStream.Count)
            {
                data_type.Children.Add(match(Token_Class.Float));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.String && InputPointer < TokenStream.Count)
            {
                data_type.Children.Add(match(Token_Class.String));
            }
            return data_type;
        }

        // Assignment_Statement → Identifier := Expression
        Node Assignment_Statement()
        {
            Node assignment_statement = new Node("Assignment_Statement");
            assignment_statement.Children.Add(match(Token_Class.Idenifier));
            assignment_statement.Children.Add(match(Token_Class.Assignment));
            // assignment_statement.Children.Add(Expresion());
            return assignment_statement;
        }

        // Read_Statement → read Identifier;
        Node Read_Statement()
        {
            Node read_rtatement = new Node("Read_Statement");
            read_rtatement.Children.Add(match(Token_Class.Read));
            read_rtatement.Children.Add(match(Token_Class.Idenifier));
            read_rtatement.Children.Add(match(Token_Class.Semicolon));
            return read_rtatement;
        }

        // Return_Statement → return Expression;
        Node Return_Statement()
        {
            Node return_statement = new Node("Return_Statement");
            return_statement.Children.Add(match(Token_Class.Return));
            // return_statement.Children.Add(Expresion());
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
        }

        Node Reserved_Words()
        {
            Node reserved_Words = new Node("Reserved_Words");
            if (TokenStream[InputPointer].token_type == Token_Class.Int && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Int));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Float && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Float));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.String && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.String));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Read && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Read));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Write && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Write));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Repeat && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Repeat));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Until && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Until));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.If && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.If));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Elseif && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Elseif));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Else && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Else));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Then && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Then));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Endl && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Endl));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Return && InputPointer < TokenStream.Count)
            {
                reserved_Words.Children.Add(match(Token_Class.Return));
            }
            return reserved_Words;
        }

        ////////////////////////////////////////////////
        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());
                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
