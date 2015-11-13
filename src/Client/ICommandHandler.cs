namespace Client
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}