<div align="center">

  <h1>VOTISE</h1>

  <p>
    <strong>Ranked Choice Voting Platform</strong><br/>
    A modern web-based voting system where voters rank options by preference.
  </p>

  <p>
    <img src="https://twemoji.maxcdn.com/v/latest/svg/1f5f3.svg" width="28" alt="ballot box"/>
    <img src="https://twemoji.maxcdn.com/v/latest/svg/1f4ca.svg" width="28" alt="chart"/>
    <img src="https://twemoji.maxcdn.com/v/latest/svg/1f9e0.svg" width="28" alt="brain"/>
    <img src="https://twemoji.maxcdn.com/v/latest/svg/1f517.svg" width="28" alt="link"/>
  </p>

</div>

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/2728.svg" width="20"/> Overview
**VOTISE** is a full-stack voting platform that implements **Ranked Choice Voting (RCV)** —  
a smarter voting method where participants rank options instead of selecting only one.

The system automatically calculates winners using elimination rounds and vote redistribution until a final winner is reached.

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f680.svg" width="20"/> Key Features
- <img src="https://twemoji.maxcdn.com/v/latest/svg/2705.svg" width="18"/> Create & manage votes (Draft / Open / Closed)
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f512.svg" width="18"/> Public & private voting modes
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f517.svg" width="18"/> Shareable voting link
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f522.svg" width="18"/> Ranked voting (1st, 2nd, 3rd…)
- <img src="https://twemoji.maxcdn.com/v/latest/svg/23f1.svg" width="18"/> Time-based auto closing
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f504.svg" width="18"/> RCV elimination rounds (redistribution each round)
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f4c8.svg" width="18"/> Round-by-round results visualization
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f464.svg" width="18"/> Authentication & user profiles

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f9e0.svg" width="20"/> Ranked Choice Voting (RCV) — How It Works
1) <img src="https://twemoji.maxcdn.com/v/latest/svg/270d.svg" width="18"/> Voters rank options by preference.  
2) <img src="https://twemoji.maxcdn.com/v/latest/svg/1f3af.svg" width="18"/> If one option gets a majority → it wins.  
3) <img src="https://twemoji.maxcdn.com/v/latest/svg/1f6ab.svg" width="18"/> If no majority → eliminate the lowest option.  
4) <img src="https://twemoji.maxcdn.com/v/latest/svg/1f500.svg" width="18"/> Redistribute those ballots to the next preferred active option.  
5) <img src="https://twemoji.maxcdn.com/v/latest/svg/1f501.svg" width="18"/> Repeat until a final winner is reached.

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f6e0.svg" width="20"/> Tech Stack

<p>
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/dotnet.svg" width="22" alt=".NET" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/csharp.svg" width="22" alt="C#" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/microsoftsqlserver.svg" width="22" alt="SQL Server" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/html5.svg" width="22" alt="HTML5" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/css3.svg" width="22" alt="CSS3" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/tailwindcss.svg" width="22" alt="Tailwind" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/docker.svg" width="22" alt="Docker" />
  <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/jetbrains.svg" width="22" alt="JetBrains" />
</p>

- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f539.svg" width="16"/> **Backend:** ASP.NET Core MVC (C#)
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f539.svg" width="16"/> **Database:** SQL Server + Entity Framework Core
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f539.svg" width="16"/> **Frontend:** Razor Views + Tailwind CSS
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f539.svg" width="16"/> **Auth:** ASP.NET Identity
- <img src="https://twemoji.maxcdn.com/v/latest/svg/1f539.svg" width="16"/> **Tools:** Rider, Docker, Azure Data Studio

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f4c2.svg" width="20"/> Project Structure
```text
VOTISE/
├─ Controllers/
├─ Models/
├─ Services/
├─ ViewModels/
├─ Views/
├─ wwwroot/
├─ Program.cs
└─ appsettings.json
```

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/2699.svg" width="20"/> Setup & Run Locally

### <img src="https://twemoji.maxcdn.com/v/latest/svg/1f4cb.svg" width="18"/> Prerequisites
- .NET SDK
- SQL Server (Docker recommended)
- EF Core Tools

### <img src="https://twemoji.maxcdn.com/v/latest/svg/1f4e5.svg" width="18"/> Clone the repo
```bash
git clone https://github.com/afnmo/rcv-voting-system.git
cd VOTISE
```

### <img src="https://twemoji.maxcdn.com/v/latest/svg/1f4dd.svg" width="18"/> Configure database
Update the connection string inside:  
`appsettings.json`

### <img src="https://twemoji.maxcdn.com/v/latest/svg/1f9f1.svg" width="18"/> Run migrations
```bash
dotnet ef database update
```

### <img src="https://twemoji.maxcdn.com/v/latest/svg/25b6.svg" width="18"/> Run the project
```bash
dotnet run
```

Then open: `https://localhost:5089`

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f9ea.svg" width="20"/> Demo Data
The project includes a seeder that generates:
- demo user
- sample votes
- ballots & rankings

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f5bc.svg" width="20"/> Screenshots
> Add screenshots here

- Home page  
- Vote creation  
- Ranking page  
- Results (Rounds)

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f6e3.svg" width="20"/> Future Improvements
- Export results (PDF / CSV)
- Real-time results dashboard
- Advanced privacy controls
- Admin panel & moderation
- Multi-language support (AR/EN)

---

## <img src="https://twemoji.maxcdn.com/v/latest/svg/1f464.svg" width="20"/> Author
**Afnan Alotaibi**  
Software Engineer  

<p>
  <a href="https://github.com/afnmo">
    <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/github.svg" width="22" alt="GitHub" />
  </a>
  <a href="https://linkedin.com/in/afnanalotaibi0">
    <img src="https://cdn.jsdelivr.net/npm/simple-icons@v9/icons/linkedin.svg" width="22" alt="LinkedIn" />
  </a>
</p>
