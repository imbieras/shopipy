import {h, render} from 'https://esm.sh/preact';
import {useState} from 'https://esm.sh/preact/hooks';
import htm from 'https://esm.sh/htm';

const html = htm.bind(h);

let logins = JSON.parse(localStorage.getItem('recentLogins'))
if (!Array.isArray(logins)) {
    logins = [];
}

const Form = ({onClose}) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [selectedLogin, setSelectedLogin] = useState('');

    const handleSubmit = async (e) => {
        console.log(e.target);  
        e.preventDefault();
        const res = await fetch(window.location.origin + '/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                username: username,
                password: password,
            },)
        })
        if (res.status !== 200) {
            alert('unauthorized');
            return;
        }
        const data = await res.json();
        window.ui.preauthorizeApiKey('Bearer', data.token);
        logins = logins.filter(l => l.username !== username);
        logins.push({username, password})
        localStorage.setItem('recentLogins', JSON.stringify(logins));
        onClose();
    }

    return html`
        <form onSubmit=${handleSubmit}>
            <div class="wrapper">
                <label>
                    Recent login:
                    <select value=${selectedLogin} onChange=${e => {
                        const selected = logins[e.target.value];
                        if (selected) {
                            setUsername(selected.username);
                            setPassword(selected.password);
                        }
                        setSelectedLogin(e.target.value);
                    }}>
                        <option value="">Select a login</option>
                        ${logins.map((login, index) => html`
                            <option value=${index}>${login.username}</option>
                        `)}
                    </select>
                </label>
            </div>
            <div class="wrapper">
                <label>
                    Username:
                    <input type="text" value=${username} onChange=${e => setUsername(e.target.value)}/>
                </label>
            </div>
            <div class="wrapper">
                <label>
                    Password:
                    <input type="password" value=${password} onChange=${e => setPassword(e.target.value)}/>
                </label>
            </div>
            <button class="btn">Login</button>
        </form>`;
}

const Popup = ({onClose}) => {
    return html`
        <div class="dialog-ux">
            <div class="backdrop-ux"></div>
            <div class="modal-ux">
                <div class="modal-dialog-ux">
                    <div class="modal-ux-inner">
                        <div class="modal-ux-header">
                            <h3>Login</h3>
                            <button class="btn" onClick=${onClose}>Close</button>
                        </div>
                        <div class="modal-ux-content">
                            <${Form} onClose=${onClose}/>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
}

const LoginArea = () => {
    const [showPopup, setShowPopup] = useState(false);

    const handleClick = () => setShowPopup(true);
    const handleClose = () => setShowPopup(false);

    return html`
        <button class="btn" onClick=${handleClick}>Login</button>
        ${showPopup && html`
            <${Popup} onClose=${handleClose}/>`}
    `;
}

const wrapperIntervalId = setInterval(() => {
    const schemesContainer = document.querySelector('.schemes');
    if (!schemesContainer) return;
    clearInterval(wrapperIntervalId);
    const wrapper = document.createElement('div');
    wrapper.classList.add('login-wrapper');
    schemesContainer.appendChild(wrapper);
    render(html`
        <${LoginArea}/>`, wrapper);
});

