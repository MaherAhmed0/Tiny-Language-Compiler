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
            function_statement.Children.Add(Function_Body());
            return function_statement;
        }

        // Function_Declaration → Datatype Function_Name (Parameters_List)
        Node Function_Declaration()
        {
            Node function_declaration = new Node("Function_Declaration");
            function_declaration.Children.Add(Data_Type());
            function_declaration.Children.Add(Function_Name());
            function_declaration.Children.Add(match(Token_Class.L_Bracket));
            function_declaration.Children.Add(Parameters_List());
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
            function_body.Children.Add(Statements()); 
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.R_Curly_Bracket));
            return function_body;
        }

        // Function_Call → Identifier(Idenifier_list)
        Node Function_Call()
        {
            Node function_call = new Node("Function_Call");
            function_call.Children.Add(match(Token_Class.Idenifier));
            function_call.Children.Add(match(Token_Class.L_Bracket));
            function_call.Children.Add(Idenifiers_List());
            function_call.Children.Add(match(Token_Class.R_Bracket));
            return function_call;
        }

        // Idenifiers_List → Identifier Iden_List
        Node Idenifiers_List()
        {
            Node idenifiers_list = new Node("Idenifiers_List");
            idenifiers_list.Children.Add(match(Token_Class.Idenifier));
            idenifiers_list.Children.Add(Iden_List());
            return idenifiers_list;
        }

        // Iden_List → , Identifier Iden_List | ε
        Node Iden_List()
        {
            Node iden_list = new Node("Iden_List");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma && InputPointer < TokenStream.Count)
            {
                iden_list.Children.Add(match(Token_Class.Comma));
                iden_list.Children.Add(match(Token_Class.Idenifier));
                iden_list.Children.Add(Iden_List());
            }
            else
            {
                return null;
            }
            return iden_list;
        }

        // Parameters_List → Parameter Par_List | ε
        Node Parameters_List()
        {
            Node parameters_list = new Node("Parameters_List");
            if ((TokenStream[InputPointer].token_type == Token_Class.Int 
                || TokenStream[InputPointer].token_type == Token_Class.Float 
                || TokenStream[InputPointer].token_type == Token_Class.String) 
                && (InputPointer < TokenStream.Count))
            {
                parameters_list.Children.Add(Parameter());
                parameters_list.Children.Add(Par_List());
            }
            else
            {
                return null;
            }
            return parameters_list;
        }

        // Parameter → Data_Type Identifier
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Data_Type());
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }

        // Par_List → , Parameters_List | ε
        Node Par_List()
        {
            Node par_list = new Node("Par_List");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma && InputPointer < TokenStream.Count)
            {
                par_list.Children.Add(match(Token_Class.Comma));
                par_list.Children.Add(Parameters_List());
            }
            else
            {
                return null;
            }
            return par_list;
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
            assignment_statement.Children.Add(Expression());
            return assignment_statement;
        }

        // Read_Statement → read Identifier;
        Node Read_Statement()
        {
            Node read_srtatement = new Node("Read_Statement");
            read_srtatement.Children.Add(match(Token_Class.Read));
            read_srtatement.Children.Add(match(Token_Class.Idenifier));
            read_srtatement.Children.Add(match(Token_Class.Semicolon));
            return read_srtatement;
        }

        // Write_Statement → write Something;
        Node Write_Statement()
        {
            Node write_srtatement = new Node("Write_Statement");
            write_srtatement.Children.Add(match(Token_Class.Write));
            write_srtatement.Children.Add(Something());
            write_srtatement.Children.Add(match(Token_Class.Semicolon));
            return write_srtatement;
        }

        // Something → Expression | endl
        Node Something()
        {
            Node something = new Node("Something");
            if (TokenStream[InputPointer].token_type == Token_Class.String 
                || TokenStream[InputPointer].token_type == Token_Class.Number 
                || TokenStream[InputPointer].token_type == Token_Class.Idenifier 
                && InputPointer < TokenStream.Count)
            {
                something.Children.Add(Expression());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                something.Children.Add(match(Token_Class.End));
            }
            return something;
        }

        // Return_Statement → return Expression;
        Node Return_Statement()
        {
            Node return_statement = new Node("Return_Statement");
            return_statement.Children.Add(match(Token_Class.Return));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
        }

        // Expression → string | Term | Equation
        Node Expression()
        {
            Node expression = new Node("Expression");
            if(TokenStream[InputPointer].token_type == Token_Class.String && InputPointer < TokenStream.Count)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier 
                || TokenStream[InputPointer].token_type == Token_Class.Number 
                && InputPointer < TokenStream.Count)
            {
                expression.Children.Add(Term());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number 
                || TokenStream[InputPointer].token_type == Token_Class.L_Bracket
                && InputPointer < TokenStream.Count)
            {
                expression.Children.Add(General_Equation());
            }
            return expression;
        }

        // Term → identifier | number | Function_Call
        Node Term()
        {
            Node term = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.L_Bracket && InputPointer + 1 < TokenStream.Count)
                {
                    term.Children.Add(Function_Call());
                }
                else 
                {
                    term.Children.Add(match(Token_Class.Idenifier));
                }
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number && InputPointer < TokenStream.Count)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            return term;
        }

        // General_Equation → Term Equation | Equation
        Node General_Equation()
        {
            Node general_equation = new Node("General_Equation");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier
                || TokenStream[InputPointer].token_type == Token_Class.Number
                && InputPointer < TokenStream.Count)
            {
                general_equation.Children.Add(Term());
                general_equation.Children.Add(Equation());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.L_Bracket && InputPointer < TokenStream.Count)
            {
                general_equation.Children.Add(Equation());
            }
            return general_equation;
        }

        // Equation → Sub_Equation_1 | Sub_Equation_2 | ε
        Node Equation()
        {
            Node equation = new Node("Equation");
            if (TokenStream[InputPointer].token_type == Token_Class.Plus 
                || TokenStream[InputPointer].token_type == Token_Class.Minus 
                || TokenStream[InputPointer].token_type == Token_Class.Multiply 
                || TokenStream[InputPointer].token_type == Token_Class.Divide
                && InputPointer < TokenStream.Count)
            {
                equation.Children.Add(Sub_Equation_1());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.L_Bracket && InputPointer < TokenStream.Count)
            {
                equation.Children.Add(Sub_Equation_2());
            }
            else
            {
                return null;
            }
            return equation;
        }

        // Sub_Equation_1 → Arithmetic_Operator Term Equation
        Node Sub_Equation_1()
        {
            Node sub_equation_1 = new Node("Sub_Equation_1");
            sub_equation_1.Children.Add(Arithmatic_Operator());
            sub_equation_1.Children.Add(Term());
            sub_equation_1.Children.Add(Equation());
            return sub_equation_1;
        }

        // Sub_Equation_2 → (Term  Sub_Equation_1) Equation | ε
        Node Sub_Equation_2()
        {
            Node sub_equation_2 = new Node("Sub_Equation_2");
            if (TokenStream[InputPointer].token_type == Token_Class.L_Bracket && InputPointer < TokenStream.Count)
            {
                sub_equation_2.Children.Add(match(Token_Class.L_Bracket));
                sub_equation_2.Children.Add(Term());
                sub_equation_2.Children.Add(Sub_Equation_1());
                sub_equation_2.Children.Add(match(Token_Class.R_Bracket));
                sub_equation_2.Children.Add(Equation());
            }
            else
            {
                return null;
            }
            return sub_equation_2;
        }

        // Condition → Identifier Condition_Operator Term
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }

        // Condition_Statement → Condition Sub_Condition 
        Node Condition_Statement()
        {
            Node condition_statement = new Node("Condition_Statement");
            condition_statement.Children.Add(Condition());
            condition_statement.Children.Add(Sub_Condition());
            return condition_statement;
        }

        // Sub_Condition → Boolean_Operator Condition Sub_Condition | ε
        Node Sub_Condition()
        {
            Node sub_condition = new Node("Sub_Condition");
            if (TokenStream[InputPointer].token_type == Token_Class.AND
                || TokenStream[InputPointer].token_type == Token_Class.OR
                && InputPointer < TokenStream.Count)
            {
                sub_condition.Children.Add(Boolean_Operator());
                sub_condition.Children.Add(Sub_Condition());
            }
            else
            {
                return null;
            }
            return sub_condition;
        }

        // Declration_Statement → DataType Identifiers;
        Node Declration_Statement()
        {
            Node declration_statement = new Node("Declration_Statement");
            declration_statement.Children.Add(Data_Type());
            declration_statement.Children.Add(Identifiers());
            return declration_statement;
        }

        // Identifiers → Identifier Assignment List_of_Identifiers
        Node Identifiers()
        {
            Node identifiers = new Node("Identifiers");
            identifiers.Children.Add(match(Token_Class.Idenifier));
            identifiers.Children.Add(Assignment());
            identifiers.Children.Add(List_of_Identifiers());
            return identifiers;
        }

        // Assignment → := Expression | ε
        Node Assignment()
        {
            Node assignment = new Node("Assignment");
            if (TokenStream[InputPointer].token_type == Token_Class.Assignment && InputPointer < TokenStream.Count)
            {
                assignment.Children.Add(match(Token_Class.Assignment));
                assignment.Children.Add(Expression());
            }
            else
            {
                return null;
            }
            return assignment;
        }

        // List_of_Identifiers → , Identifiers | ε
        Node List_of_Identifiers()
        {
            Node list_of_identifiers = new Node("List_of_Identifiers");
            if(TokenStream[InputPointer].token_type == Token_Class.Comma && InputPointer < TokenStream.Count)
            {
                list_of_identifiers.Children.Add(match(Token_Class.Comma));
                list_of_identifiers.Children.Add(Identifiers());
            }
            else
            {
                return null;
            }
            return list_of_identifiers;
        }

        // If_Statment → If Condition_Statement Then Statements ELSE_Statments
        Node If_Statement()
        {
            Node if_statement = new Node("If_Statment");
            if_statement.Children.Add(match(Token_Class.If));
            if_statement.Children.Add(Condition_Statement());
            if_statement.Children.Add(match(Token_Class.Then));
            if_statement.Children.Add(Statements());
            if_statement.Children.Add(ELSE_Statements());
            return if_statement;
        }

        // ELSE_Statments → Else_If_Statment | Else_Statment | End
        Node ELSE_Statements()
        {
            Node ELSE_statements = new Node("ELSE_Statments");
            if (TokenStream[InputPointer].token_type == Token_Class.Elseif && InputPointer < TokenStream.Count)
            {
                ELSE_statements.Children.Add(Else_If_Statement());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.Else && InputPointer < TokenStream.Count)
            {
                ELSE_statements.Children.Add(Else_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.End && InputPointer < TokenStream.Count)
            {
                ELSE_statements.Children.Add(match(Token_Class.End));
            }
            return ELSE_statements;
        }

        // Else_If_Statment → ElseIf Condition_Statement Then Statements
        //                    ELSE_Statments
        Node Else_If_Statement()
        {
            Node else_if_statement = new Node("Else_If_Statment");
            else_if_statement.Children.Add(match(Token_Class.Elseif));
            else_if_statement.Children.Add(Condition_Statement());
            else_if_statement.Children.Add(match(Token_Class.Then));
            else_if_statement.Children.Add(Statements());
            else_if_statement.Children.Add(ELSE_Statements());
            return else_if_statement;
        }

        // Else_Statment → Else Statements End
        Node Else_Statement()
        {
            Node else_statment = new Node("Else_Statment");
            else_statment.Children.Add(match(Token_Class.Else));
            else_statment.Children.Add(Statements());
            else_statment.Children.Add(match(Token_Class.End));
            return else_statment;
        }

        // Repeat_Statement → Repeat Statements Until Condition_Statement
        Node Repeat_Statement()
        {
            Node repeat_statement = new Node("Repeat_Statement");
            repeat_statement.Children.Add(match(Token_Class.Repeat));
            repeat_statement.Children.Add(Statements());
            repeat_statement.Children.Add(match(Token_Class.Until));
            repeat_statement.Children.Add(Condition_Statement());
            return repeat_statement;
        }

        // Statments → Statment State
        Node Statements()
        {
            Node statements = new Node("Statements ");
            //statements.Children.Add(Statement());
            //statements.Children.Add(State());
            Statement();
            State();
            return statements;
        }

        // Statement → Assignment_Statement | Declaration_Statement |
        //             Write_Statement | Read_Statement | If_Statement |
        //             Repeat_Statement | Function_Statement | Function_Call
        //             Condition_Statement | ε
        Node Statement()
        {
            Node statement = new Node("Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Int
                || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String
                && InputPointer < TokenStream.Count)
            {
                statement.Children.Add(Declration_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Write && InputPointer < TokenStream.Count)
            {
                statement.Children.Add(Write_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Read && InputPointer < TokenStream.Count)
            {
                statement.Children.Add(Read_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.If && InputPointer < TokenStream.Count)
            {
                statement.Children.Add(If_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Repeat && InputPointer < TokenStream.Count)
            {
                statement.Children.Add(Repeat_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.Assignment && InputPointer + 1 < TokenStream.Count)
                {
                    statement.Children.Add(Assignment_Statement());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                else if (TokenStream[InputPointer + 1].token_type == Token_Class.L_Bracket && InputPointer + 1 < TokenStream.Count)
                {
                    statement.Children.Add(Function_Call());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                else if(TokenStream[InputPointer + 1].token_type == Token_Class.Equal
                        || TokenStream[InputPointer + 1].token_type == Token_Class.GreaterThan
                        || TokenStream[InputPointer + 1].token_type == Token_Class.LessThan
                        || TokenStream[InputPointer + 1].token_type == Token_Class.NotEqual
                        && InputPointer + 1 < TokenStream.Count)
                {
                    statement.Children.Add(Condition_Statement());
                }
            }
            return statement;
        }

        // State → ; Statement State | ε
        Node State()
        {
            Node state = new Node("State");
            if (TokenStream[InputPointer].token_type == Token_Class.Semicolon && InputPointer < TokenStream.Count)
            {
                state.Children.Add(match(Token_Class.Semicolon));
                state.Children.Add(Statement());
                state.Children.Add(State());

            }
            else
            {
                return null;
            }
            return state;
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
