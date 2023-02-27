$('#login-form').submit(function (event) {
    event.preventDefault();
    var email = $('#email').val();
    var password = $('#password').val();
    var data = {
        email: email,
        password: password
    };
    $.ajax({
        url: 'https://localhost:7022/User/Login',
        type: 'POST',
        contentType: 'application/json-patch+json',
        data: JSON.stringify(data),
        success: function (response) {
            alert("Login successfully!")
            localStorage.setItem('token', response);
            //window.location.href = 'https://localhost:7233/Movies/Index';
        },
        error: function (response) {
            alert("Login failed: " + response.responseJSON)
        }
    });
});

