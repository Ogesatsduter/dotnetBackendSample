namespace Backend.Settings;

public class ResponseSettings
{
    public string UsernameAlreadyTaken { get; set; }
    public string PasswordOrUsernameWrong { get; set; }
    public string CannotTerminateAccountOfOtherUser { get; set; }
    public string NotLoggedIn { get; set; }


    public string CouldNotLogIn { get; set; }
    public string UserNotFound { get; set; }
    public string UserDoesNotExist { get; set; }
    public string CouldNotRemoveUser { get; set; }
    public string CannotAddSelfAsContributor { get; set; }
    public string UserAlreadyContributesToList { get; set; }
    public string UserDoesNotContributeToList { get; set; }
    public string ListDoesNotBelongToUser { get; set; }
    public string TodoListDoesNotExist { get; set; }
    public string CouldNotFindTodoList { get; set; }
    public string NoRightToDeleteTodoList { get; set; }
    public string CouldNotFindTodo { get; set; }
    public string CouldNotFindTodoToDelete { get; set; }
    public string RemovedTodoList { get; set; }
    public string CouldNotRemoveTodoList { get; set; }
    public string CouldNotAddContributor { get; set; }
    public string CouldNotRemoveContributor { get; set; }
    public string CouldNotUpdateTodoList { get; set; }
    public string CouldNotCreateTodoList { get; set; }
    public string ErrorTryingSaveNewTodoList { get; set; }
}