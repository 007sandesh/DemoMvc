import { useState, useEffect } from 'react'
import Login from './Login';
import Register from './Register';
import './App.css'

function App() {
  // --- AUTH STATE ---
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [role, setRole] = useState(localStorage.getItem('role'));
  const [username, setUsername] = useState(localStorage.getItem('username'));
  const [authView, setAuthView] = useState('login');

  // --- APP STATE ---
  const [view, setView] = useState('students'); // 'students', 'courses', 'teachers', 'lessons', 'assignments'
  const [students, setStudents] = useState([]);
  const [courses, setCourses] = useState([]);
  const [teachers, setTeachers] = useState([]);

  // --- LESSONS STATE ---
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [lessons, setLessons] = useState([]);
  const [newLesson, setNewLesson] = useState({ title: '', contentType: 'Video', file: null });

  // --- ASSIGNMENTS STATE ---
  const [assignments, setAssignments] = useState([]);
  const [newAssignment, setNewAssignment] = useState({ title: '', description: '', dueDate: '' });
  const [submissionFile, setSubmissionFile] = useState(null);
  const [submissions, setSubmissions] = useState([]); // For teachers to view
  const [selectedAssignmentId, setSelectedAssignmentId] = useState(null); // For viewing submissions
  const [gradeData, setGradeData] = useState({ grade: 0, feedback: '' });

  // --- PROGRESS STATE ---
  const [courseProgress, setCourseProgress] = useState({}); // {courseId: {percentage, completed, total}}
  const [lessonCompletion, setLessonCompletion] = useState({}); // {lessonId: boolean}

  // --- FORM STATE ---
  const [newStudent, setNewStudent] = useState({ firstName: '', lastName: '', email: '', phoneNumber: '' });
  const [newCourse, setNewCourse] = useState({ title: '', credits: 3, description: '', teacherId: '' });
  const [newTeacher, setNewTeacher] = useState({ firstName: '', lastName: '', email: '', specialization: '' });
  const [enrollment, setEnrollment] = useState({ studentId: '', courseId: '' });

  // --- INITIAL LOAD ---
  useEffect(() => {
    if (token) {
      fetchStudents();
      fetchCourses();
      fetchTeachers();
    }
  }, [token]);

  // --- AUTH HELPERS ---
  const handleLogin = (data) => {
    setToken(data.token);
    setRole(data.role);
    setUsername(data.username);
  };

  const handleLogout = () => {
    localStorage.clear();
    setToken(null);
    setRole(null);
    setUsername(null);
    setAuthView('login');
  };

  const getAuthHeaders = () => ({ 'Authorization': `Bearer ${token}` });
  const getJsonHeaders = () => ({ 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` });

  // --- API CALLS ---
  const fetchStudents = () => fetch('https://localhost:7015/api/students', { headers: getJsonHeaders() }).then(res => res.json()).then(setStudents);
  const fetchCourses = () => fetch('https://localhost:7015/api/courses', { headers: getJsonHeaders() }).then(res => res.json()).then(setCourses);
  const fetchTeachers = () => fetch('https://localhost:7015/api/teachers', { headers: getJsonHeaders() }).then(res => res.json()).then(setTeachers);

  const fetchLessons = (courseId) => {
    fetch(`https://localhost:7015/api/courses/${courseId}/lessons`, { headers: getJsonHeaders() })
      .then(res => res.json())
      .then(data => {
        setLessons(data);
        if (role === 'Student') {
          // Fetch completion status for each lesson
          data.forEach(lesson => fetchLessonCompletion(lesson.id));
        }
      });
  };

  const fetchAssignments = (courseId) => fetch(`https://localhost:7015/api/courses/${courseId}/assignments`, { headers: getJsonHeaders() }).then(res => res.json()).then(setAssignments);

  const fetchSubmissions = (assignmentId) => {
    fetch(`https://localhost:7015/api/assignments/${assignmentId}/submissions`, { headers: getJsonHeaders() })
      .then(res => res.json())
      .then(setSubmissions);
  };

  const fetchCourseProgress = (courseId) => {
    if (role !== 'Student') return;
    fetch(`https://localhost:7015/api/progress/courses/${courseId}`, { headers: getJsonHeaders() })
      .then(res => res.json())
      .then(data => setCourseProgress(prev => ({ ...prev, [courseId]: data })));
  };

  const fetchLessonCompletion = (lessonId) => {
    fetch(`https://localhost:7015/api/progress/lessons/${lessonId}`, { headers: getJsonHeaders() })
      .then(res => res.json())
      .then(data => setLessonCompletion(prev => ({ ...prev, [lessonId]: data.isCompleted })));
  };

  const toggleLessonCompletion = (lessonId) => {
    fetch(`https://localhost:7015/api/progress/lessons/${lessonId}/toggle`, {
      method: 'POST',
      headers: getJsonHeaders()
    }).then(res => res.json())
      .then(data => {
        setLessonCompletion(prev => ({ ...prev, [lessonId]: data.isCompleted }));
        if (selectedCourse) fetchCourseProgress(selectedCourse.id);
      });
  };

  // --- ACTION HANDLERS ---
  const handleViewLessons = (course) => {
    setSelectedCourse(course);
    fetchLessons(course.id);
    if (role === 'Student') fetchCourseProgress(course.id);
    setView('lessons');
  };

  const handleViewAssignments = (course) => {
    setSelectedCourse(course);
    fetchAssignments(course.id);
    setView('assignments');
    setSubmissions([]); // Clear previous
    setSelectedAssignmentId(null);
  };

  const handleAddAssignment = (e) => {
    e.preventDefault();
    const payload = { ...newAssignment, courseId: selectedCourse.id, dueDate: new Date(newAssignment.dueDate) };
    fetch('https://localhost:7015/api/assignments', {
      method: 'POST',
      headers: getJsonHeaders(),
      body: JSON.stringify(payload)
    }).then(res => {
      if (res.ok) {
        fetchAssignments(selectedCourse.id);
        setNewAssignment({ title: '', description: '', dueDate: '' });
        alert("Assignment Created!");
      }
    });
  };

  const handleSubmitAssignment = (assignmentId) => {
    if (!submissionFile) return alert("Please select a file.");
    const formData = new FormData();
    formData.append('file', submissionFile);

    fetch(`https://localhost:7015/api/assignments/${assignmentId}/submit`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: formData
    }).then(res => {
      if (res.ok) {
        alert("Submitted Successfully!");
        setSubmissionFile(null);
      } else {
        alert("Submission Failed.");
      }
    });
  };

  const handleGradeSubmission = (submissionId) => {
    fetch(`https://localhost:7015/api/assignments/submissions/${submissionId}/grade`, {
      method: 'POST',
      headers: getJsonHeaders(),
      body: JSON.stringify(gradeData)
    }).then(res => {
      if (res.ok) {
        alert("Graded!");
        fetchSubmissions(selectedAssignmentId); // Refresh
      }
    });
  };

  // --- RENDER AUTH VIEWS ---
  if (!token) {
    return (
      <div className="container">
        <div className="header"><h1>🎓 MiniLMS</h1><p>Please login to continue.</p></div>
        {authView === 'login' ? <Login onLogin={handleLogin} onSwitchToRegister={() => setAuthView('register')} /> : <Register onRegisterSuccess={() => setAuthView('login')} onSwitchToLogin={() => setAuthView('login')} />}
      </div>
    );
  }

  // --- RENDER MAIN APP ---
  return (
    <div className="container">
      <div className="header">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div><h1>🎓 MiniLMS Dashboard</h1><p>Welcome, <strong>{username}</strong> ({role})</p></div>
          <button onClick={handleLogout} className="secondary-btn">Logout</button>
        </div>
      </div>

      <div className="tabs">
        <button className={`tab-btn ${view === 'students' ? 'active' : ''}`} onClick={() => setView('students')}>Students</button>
        <button className={`tab-btn ${view === 'teachers' ? 'active' : ''}`} onClick={() => setView('teachers')}>Teachers</button>
        <button className={`tab-btn ${view === 'courses' ? 'active' : ''}`} onClick={() => setView('courses')}>Courses</button>
      </div>

      {/* --- STUDENTS & TEACHERS VIEWS (Simplified for brevity, same as before) --- */}
      {view === 'students' && <div className="fade-in"><h3>Student List ({students.length})</h3><ul className="list">{students.map(s => <li key={s.id} className="list-item"><strong>{s.firstName} {s.lastName}</strong></li>)}</ul></div>}
      {view === 'teachers' && <div className="fade-in"><h3>Teacher List ({teachers.length})</h3><ul className="list">{teachers.map(t => <li key={t.id} className="list-item"><strong>{t.firstName} {t.lastName}</strong></li>)}</ul></div>}

      {/* --- COURSES VIEW --- */}
      {view === 'courses' && (
        <div className="fade-in">
          <h3>Course List ({courses.length})</h3>
          <ul className="list">
            {courses.map(c => (
              <li key={c.id} className="list-item">
                <div className="item-info">
                  <strong>{c.title}</strong>
                  {role === 'Student' && courseProgress[c.id] && (
                    <div style={{ marginTop: '8px' }}>
                      <div style={{ background: '#e5e7eb', borderRadius: '8px', height: '8px', overflow: 'hidden' }}>
                        <div style={{ background: '#4f46e5', height: '100%', width: `${courseProgress[c.id].percentage}%`, transition: 'width 0.3s' }}></div>
                      </div>
                      <span style={{ fontSize: '12px', color: '#666' }}>{courseProgress[c.id].percentage}% Complete ({courseProgress[c.id].completed}/{courseProgress[c.id].total} lessons)</span>
                    </div>
                  )}
                  <div style={{ marginTop: '10px' }}>
                    <button onClick={() => handleViewLessons(c)} className="secondary-btn" style={{ marginRight: '10px' }}>📂 Content</button>
                    <button onClick={() => handleViewAssignments(c)} className="primary-btn">📝 Assignments</button>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* --- LESSONS VIEW --- */}
      {view === 'lessons' && selectedCourse && (
        <div className="fade-in">
          <button onClick={() => setView('courses')} className="secondary-btn">← Back</button>
          <h2>📂 Content: {selectedCourse.title}</h2>
          {role === 'Student' && courseProgress[selectedCourse.id] && (
            <div style={{ background: '#f3f4f6', padding: '15px', borderRadius: '8px', marginBottom: '20px' }}>
              <h4 style={{ margin: '0 0 10px 0' }}>📊 Your Progress</h4>
              <div style={{ background: '#e5e7eb', borderRadius: '8px', height: '12px', overflow: 'hidden' }}>
                <div style={{ background: '#10b981', height: '100%', width: `${courseProgress[selectedCourse.id].percentage}%`, transition: 'width 0.3s' }}></div>
              </div>
              <p style={{ margin: '8px 0 0 0', fontSize: '14px', color: '#666' }}>{courseProgress[selectedCourse.id].completed} of {courseProgress[selectedCourse.id].total} lessons completed ({courseProgress[selectedCourse.id].percentage}%)</p>
            </div>
          )}
          <ul className="list">{lessons.map(l => (
            <li key={l.id} className="list-item">
              {role === 'Student' && (
                <input
                  type="checkbox"
                  checked={lessonCompletion[l.id] || false}
                  onChange={() => toggleLessonCompletion(l.id)}
                  style={{ marginRight: '10px', width: '18px', height: '18px', cursor: 'pointer' }}
                />
              )}
              <a href={`https://localhost:7015${l.fileUrl}`} target="_blank" style={{ textDecoration: lessonCompletion[l.id] ? 'line-through' : 'none', opacity: lessonCompletion[l.id] ? 0.6 : 1 }}>🔗 {l.title}</a>
            </li>
          ))}</ul>
        </div>
      )}

      {/* --- ASSIGNMENTS VIEW --- */}
      {view === 'assignments' && selectedCourse && (
        <div className="fade-in">
          <button onClick={() => setView('courses')} className="secondary-btn">← Back</button>
          <h2>📝 Assignments: {selectedCourse.title}</h2>

          {/* Teacher: Create Assignment */}
          {(role === 'Admin' || role === 'Teacher') && (
            <div className="card">
              <h3>➕ New Assignment</h3>
              <form onSubmit={handleAddAssignment} className="form-grid">
                <input type="text" placeholder="Title" value={newAssignment.title} onChange={e => setNewAssignment({ ...newAssignment, title: e.target.value })} required />
                <input type="text" placeholder="Description" value={newAssignment.description} onChange={e => setNewAssignment({ ...newAssignment, description: e.target.value })} />
                <input type="date" value={newAssignment.dueDate} onChange={e => setNewAssignment({ ...newAssignment, dueDate: e.target.value })} required />
                <button type="submit" className="primary-btn">Create</button>
              </form>
            </div>
          )}

          {/* List Assignments */}
          <ul className="list">
            {assignments.map(a => (
              <li key={a.id} className="list-item" style={{ display: 'block' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                  <strong>{a.title}</strong>
                  <span className="badge">Due: {new Date(a.dueDate).toLocaleDateString()}</span>
                </div>
                <p>{a.description}</p>

                {/* Student: Submit */}
                {role === 'Student' && (
                  <div style={{ marginTop: '10px', padding: '10px', background: '#f9fafb', borderRadius: '8px' }}>
                    <h4>📤 Submit Work</h4>
                    <input type="file" onChange={e => setSubmissionFile(e.target.files[0])} />
                    <button onClick={() => handleSubmitAssignment(a.id)} className="primary-btn" style={{ marginLeft: '10px' }}>Submit</button>
                  </div>
                )}

                {/* Teacher: View Submissions */}
                {(role === 'Admin' || role === 'Teacher') && (
                  <div style={{ marginTop: '10px' }}>
                    <button onClick={() => { setSelectedAssignmentId(a.id); fetchSubmissions(a.id); }} className="secondary-btn">👀 View Submissions</button>
                  </div>
                )}
              </li>
            ))}
          </ul>

          {/* Teacher: Grading Interface */}
          {selectedAssignmentId && (role === 'Admin' || role === 'Teacher') && (
            <div className="card" style={{ marginTop: '20px', border: '2px solid #4f46e5' }}>
              <h3>🎓 Grading Submissions</h3>
              <ul className="list">
                {submissions.map(s => (
                  <li key={s.id} className="list-item">
                    <div>
                      <strong>Student: {s.student?.firstName} {s.student?.lastName}</strong>
                      <br />
                      <a href={`https://localhost:7015${s.fileUrl}`} target="_blank">📄 View File</a>
                      <br />
                      <span>Submitted: {new Date(s.submissionDate).toLocaleString()}</span>
                    </div>
                    <div>
                      {s.grade ? (
                        <span className="badge" style={{ background: '#10b981', color: 'white' }}>Grade: {s.grade}</span>
                      ) : (
                        <div style={{ display: 'flex', gap: '5px' }}>
                          <input type="number" placeholder="0-100" onChange={e => setGradeData({ ...gradeData, grade: e.target.value })} style={{ width: '60px' }} />
                          <input type="text" placeholder="Feedback" onChange={e => setGradeData({ ...gradeData, feedback: e.target.value })} />
                          <button onClick={() => handleGradeSubmission(s.id)} className="primary-btn">Grade</button>
                        </div>
                      )}
                    </div>
                  </li>
                ))}
                {submissions.length === 0 && <p>No submissions yet.</p>}
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default App;
