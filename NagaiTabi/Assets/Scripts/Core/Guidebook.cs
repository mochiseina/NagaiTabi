using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Guía navegable por páginas (estilo libro) para el panel Guide.
/// Muestra una página a la vez en un TextMeshProUGUI, con flechas ◀ ▶ para pasar.
///
/// SETUP:
/// 1) Pon este componente en el panel Guide.
/// 2) Arrastra el TextMeshProUGUI del cuerpo (bodyText) y, si quieres, el del título (titleText)
///    y el del contador (pageCounterText, ej. "3 / 14").
/// 3) Arrastra los botones Prev (◀) y Next (▶).
/// 4) Las páginas se editan abajo, en la lista 'pages' del Inspector, o en BuildPages().
///
/// El texto usa rich text de TMP (<b>, <size>, etc.), así que puedes dar formato.
/// </summary>
public class GuideBook : MonoBehaviour
{
	[System.Serializable]
	public class Page
	{
		public string title;
		[TextArea(5, 20)] public string body;
		[TextArea(1, 4)] public string yuinaTip; // opcional; se muestra destacado al final
	}

	[Header("Referencias UI")]
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI bodyText;
	[SerializeField] private TextMeshProUGUI pageCounterText;
	[SerializeField] private Button prevButton;
	[SerializeField] private Button nextButton;

	[Header("Formato del YuinaTip")]
	[Tooltip("Prefijo que se antepone al tip (puedes usar rich text).")]
	[SerializeField] private string yuinaTipPrefix = "<b>YuinaTip:</b> ";
	[Tooltip("Color del YuinaTip en hex (rich text).")]
	[SerializeField] private string yuinaTipColor = "#E8A0BF";

	[Header("Páginas")]
	[SerializeField] private List<Page> pages = new();

	private int currentIndex = 0;

	private void Awake()
	{
		// Si no rellenaste páginas en el Inspector, carga las de por defecto.
		if (pages == null || pages.Count == 0)
			pages = BuildDefaultPages();

		if (prevButton != null) prevButton.onClick.AddListener(PrevPage);
		if (nextButton != null) nextButton.onClick.AddListener(NextPage);
	}

	private void OnEnable()
	{
		// Al abrir el panel, muestra la página actual (o la primera).
		ShowPage(currentIndex);
	}

	public void NextPage()
	{
		if (currentIndex < pages.Count - 1)
			ShowPage(currentIndex + 1);
	}

	public void PrevPage()
	{
		if (currentIndex > 0)
			ShowPage(currentIndex - 1);
	}

	private void ShowPage(int index)
	{
		if (pages == null || pages.Count == 0) return;
		currentIndex = Mathf.Clamp(index, 0, pages.Count - 1);
		var page = pages[currentIndex];

		if (titleText != null)
			titleText.text = page.title;

		if (bodyText != null)
		{
			string text = page.body;
			if (!string.IsNullOrWhiteSpace(page.yuinaTip))
			{
				text += $"\n\n<color={yuinaTipColor}>{yuinaTipPrefix}{page.yuinaTip}</color>";
			}
			bodyText.text = text;
		}

		if (pageCounterText != null)
			pageCounterText.text = $"{currentIndex + 1} / {pages.Count}";

		// Deshabilita las flechas en los extremos.
		if (prevButton != null) prevButton.interactable = currentIndex > 0;
		if (nextButton != null) nextButton.interactable = currentIndex < pages.Count - 1;
	}

	/// <summary>Páginas por defecto (resumidas). Puedes reemplazarlas en el Inspector.</summary>
	private List<Page> BuildDefaultPages()
	{
		return new List<Page>
		{
			new Page {
				title = "The Long Journey",
				body = "This guide explains what happens when you learn a language by living inside it — and why that slow, quiet process eventually becomes fluency.\n\nThe train moves on one fuel: the hours you spend understanding your target language. That's what we track, because it's the most reliable part of learning, and the part most often abandoned too early.",
				yuinaTip = "You don't have to believe me yet. Just walk a little further down the line, and let the scenery convince you."
			},
			new Page {
				title = "What Immersion Really Is",
				body = "Immersion means spending time understanding real messages in your target language: shows, books, games, podcasts.\n\nThe core idea, backed by decades of research: <b>you acquire a language by understanding things said in it.</b> Not by memorizing rules — those support you, but comprehension is the engine.\n\nWhen you understand a message, your brain quietly absorbs patterns, grammar, and sound-meaning links, without conscious 'study'."
			},
			new Page {
				title = "Why It Feels Like Nothing Happens",
				body = "The hardest truth of immersion: <b>acquisition is invisible while it's happening.</b>\n\nYou'll spend weeks understanding almost nothing, then months understanding a little. The growth is real, but it happens below the surface — like roots before a plant breaks the soil.\n\nThe map, the stations, the logged hours exist to make the invisible visible, so you can see you're moving even when it doesn't feel like it."
			},
			new Page {
				title = "The Honest Version",
				body = "Some communities say input is the ONLY thing that matters. That's a simplification.\n\nUnderstanding input is the central driver — the thing you can't skip or fake. But language is ultimately an active, social skill. Speaking, writing, feedback, and review all have their place, usually more so later.\n\nNagaiTabi tracks input because it's the foundation and the most measurable part of the journey — not because nothing else exists.",
				yuinaTip = "Input builds the engine. The rest of the journey is learning to drive. One comes before the other — that's all."
			},
			new Page {
				title = "Phase 1 — The i+1 Principle",
				body = "Aim for material <b>slightly above your current level</b> — not far above, not below.\n\nToo easy, and there's nothing to acquire. Too hard, and it's just noise. Just right, and your brain has something to reach for while standing on solid ground.\n\nIn practice, i+1 is a feeling: you understand MOST of what's happening and let the rest wash over you."
			},
			new Page {
				title = "Phase 1 — Courage to Not Understand",
				body = "At the start, you won't understand much. This is normal and necessary.\n\nYour job isn't to understand everything — it's to understand SOMETHING, and build tolerance for ambiguity. Let unknown words pass. Follow the story through images, tone, and context.\n\nThe comprehension you manage is the seed. The confusion you sit through is the soil.",
				yuinaTip = "For Japanese, learn hiragana and katakana first. A weekend of work that pays off for the whole journey."
			},
			new Page {
				title = "Phase 1 — Building a Habit",
				body = "Phase 1 is where most journeys end, because progress is least visible here. The antidote isn't intensity — it's <b>consistency.</b>\n\nA little every day beats a lot once a week. Short, regular sessions keep the language present and let your brain consolidate between them.",
				yuinaTip = "Fifteen honest minutes today beats three hours you're dreading. The streak matters more than the size."
			},
			new Page {
				title = "Phase 2 — Volume Is the Accelerator",
				body = "Once you can follow simple content, the biggest lever is <b>quantity of comprehensible input.</b>\n\nMore hours understanding the language means more encounters with words, more exposure to patterns, more chances for acquisition.\n\nThis is why the tracker counts hours. At this stage, hours ARE the work. The learners who break through are the ones who accumulated more understood input."
			},
			new Page {
				title = "Phase 2 — Intensive vs. Extensive",
				body = "<b>Extensive:</b> large amounts you mostly understand, for enjoyment. This is the bulk of your hours — it builds intuition, speed, and endurance.\n\n<b>Intensive:</b> smaller amounts you study closely — looking up words, rewatching, breaking down sentences. Slower, deeper.\n\nUse mostly extensive with regular pinches of intensive. Too much intensive burns you out; too little slows vocabulary."
			},
			new Page {
				title = "Phase 2 — Spaced Repetition",
				body = "Most words you'll acquire naturally through repeated encounters in context. But some deserve deliberate review, and here the research is strong: <b>spaced repetition works.</b>\n\nReviewing at increasing intervals — a day, then days, then a week — beats cramming. The reason is the effort of recall: each time you almost forget and then retrieve, the memory strengthens.\n\nReviewing right after studying feels effective but does little. The gap is the point.",
				yuinaTip = "Don't memorize the whole dictionary. Pull out the words you keep bumping into, and let the rest arrive on their own."
			},
			new Page {
				title = "Phase 3 — Recognition to Production",
				body = "Acquisition moves in stages: <b>recognition</b> (you know it when you see it) → <b>comprehension</b> (you understand instantly, no translating) → <b>production</b> (you can use it).\n\nThese arrive in order, and comprehension leads production. You'll understand far more than you can say for a long time — that gap isn't failure, it's the normal architecture of language."
			},
			new Page {
				title = "Phase 3 — When to Start Producing",
				body = "There's no universal 'correct' moment to start speaking. But two things are true:\n\nForcing output too early is frustrating — you can't produce patterns you haven't absorbed. And producing eventually matters: it reveals what you don't know, sharpening your attention when you return to input.\n\nLet input build the foundation, then produce when you feel the pull — or have a reason to.",
				yuinaTip = "You'll know it's time when the words start pushing to get out. Don't force the door — but don't lock it either."
			},
			new Page {
				title = "Phase 4 — Refinement",
				body = "By now you understand a lot. Refinement makes it <b>fast and automatic</b> — closing the gap between hearing and knowing. This comes from continued volume.\n\nIf speaking well matters, pronunciation deserves deliberate attention — listening closely, imitating, recording yourself. Effortful, but it's the difference between 'understandable' and 'natural'."
			},
			new Page {
				title = "Phase 4 — Maintenance Is Forever",
				body = "A language isn't a summit you reach and leave. It's a place you live.\n\nMaintenance means continuing to use it — which, if you chose content you enjoy, isn't a chore. By this stage, immersion has stopped being 'study' and become part of your life.",
				yuinaTip = "One day you'll realize you're not studying anymore. You're just... living in it. That's the whole point."
			},
		};
	}
}