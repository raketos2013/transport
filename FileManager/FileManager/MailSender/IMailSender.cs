﻿using FileManager.Domain.Entity;

namespace FileManagerServer.MailSender;

public interface IMailSender
{
    void Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files);
}
