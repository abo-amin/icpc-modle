//document.addEventListener("DOMContentLoaded", function () {
//    let currentQuestionIndex = 0;
//    let questions = [];

//    fetch("/api/exam/questions")
//        .then(response => response.json())
//        .then(data => {
//            questions = data;
//            showQuestion();
//        });

//    function showQuestion() {
//        if (currentQuestionIndex >= questions.length) {
//            document.getElementById("exam-container").innerHTML = "<h2>شكرًا لإتمام الامتحان!</h2>";
//            return;
//        }

//        const question = questions[currentQuestionIndex];
//        document.getElementById("question-container").innerHTML = `
//            <h2>${question.text}</h2>
//            ${question.choices.split(';').map(choice => `<button onclick="submitAnswer('${choice}')">${choice}</button>`).join('<br>')}
//        `;
//    }

//    function submitAnswer(answer) {
//        currentQuestionIndex++;
//        showQuestion();
//    }

//    document.getElementById("next-button").addEventListener("click", showQuestion);
//});
document.addEventListener("DOMContentLoaded", function () {
    var form = document.getElementById("exam-form");
    if (form) {
        form.addEventListener("submit", function () {
            form.querySelector("button").disabled = true;
        });
    }
});
