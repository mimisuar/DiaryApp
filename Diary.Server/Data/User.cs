﻿using Microsoft.AspNetCore.Identity;

namespace Diary.Server.Data
{
	public class User : IdentityUser
	{
		public string EncryptedKey { get; set; }
	}
}
