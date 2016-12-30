namespace BusinessLayer
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}